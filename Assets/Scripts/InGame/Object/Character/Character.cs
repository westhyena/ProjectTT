using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public enum State
    {
        Init,  // 등장
        Idle,  // 기본 상태.
        Move,  // 적을 찾아 이동 중
        Target,  // 적이 타겟된 상태
        Attack,
        Dying,
        Dead,

        Manual,
        Follow,
    }

    public enum LookDirection
    {
        Left,
        Right
    }

    protected State state = State.Init;
    protected LookDirection lookingDirection = LookDirection.Left;
    protected float curStateTime = 0.0f;

    public Vector2 Position2D { get { 
        return new Vector2(
            transform.position.x,
            transform.position.y
        );
    } }

    CharacterDataElement characterInfo;
    public CharacterDataElement CharacterInfo { get { return characterInfo; } }
    protected int characterLevel = 1;
    public int CharacterLevel { get { return characterLevel; } }

    GameObject normalHitPrefab;
    GameObject normalSkillPrefab;

    List<Skill> skillList = new ();

    protected Animator animator;
    protected string[] attackTriggers;
    protected Vector3 animatorScale;
    protected Collider2D collider2d;
    protected new Rigidbody2D rigidbody2D;

    protected float initTime = 1.0f;

    // Data Table에서 가져올 값들.
    float mspd = 500.0f; 
    float rangeOfTarget = 3.0f;
    public float RangeOfTarget { get { return rangeOfTarget; } }

    float attackSpeed = 1.0f;

    public float AttackStat { get {
        int growHP = 0, growAttackDamge = 0, growPD = 0, growMD = 0;
        DataMgr.instance.GetCharacterGrowData(
            characterInfo.ID,
            characterLevel,
            ref growHP,
            ref growAttackDamge,
            ref growPD,
            ref growMD 
        );
        return (
            characterInfo.AttackDamage + 
            growAttackDamge
        );
    } }
    float basePhysicDefenceStat = 5.0f;
    public float PhysicDefenceStat { get { return basePhysicDefenceStat; } }
    float baseMagicDefenceStat = 5.0f;
    public float MagicDefenceStat { get { return baseMagicDefenceStat; } }

    protected float movementSpeed = 20.0f;

    protected Character moveTarget = null;
    protected float targetStartDistance = 20.0f;

    public enum AttackType
    {
        Melee,
        Range,
    }
    [SerializeField]
    protected AttackType attackType = AttackType.Melee;

    GameObject projectilePrefab;
    public Transform projectileSpawnPoint;

    protected float attackStartDistance = 5.0f;
    protected float attackCooltime = 3.0f;
    protected float attackDelay = 0.2f;

    protected float dyingDelay = 3.0f;

    protected Character target = null;

    protected float hp = 100.0f;
    public float MaxHP { get
        {
            int growHP = 0, growAttackDamge = 0, growPD = 0, growMD = 0;
            DataMgr.instance.GetCharacterGrowData(
                characterInfo.ID,
                characterLevel,
                ref growHP,
                ref growAttackDamge,
                ref growPD,
                ref growMD 
            );
            return (
                characterInfo.HP + 
                growHP
            );
        }
    }
    public float HPRatio { get { return hp / MaxHP; } }
    public bool IsDead { get { return hp <= 0.0f; } }

    List<SkillEffect> skillEffectList = new ();

    protected virtual void Awake()
    {
        this.animator = GetComponentInChildren<Animator>();
        List<string> attackTriggerList = new ();
        foreach (AnimatorControllerParameter parameter in this.animator.parameters)
        {
            if (parameter.name.StartsWith("attack") && parameter.type == AnimatorControllerParameterType.Trigger)
            {
                attackTriggerList.Add(parameter.name);
            }
        }
        this.attackTriggers = attackTriggerList.ToArray();
        this.animatorScale = animator.transform.localScale;
        this.collider2d = GetComponent<Collider2D>();
        this.rigidbody2D = GetComponent<Rigidbody2D>();

        if (this.collider2d.GetType() == typeof(CircleCollider2D))
        {
            CircleCollider2D circleCollider = (CircleCollider2D)this.collider2d;
            circleCollider.radius = GameManager.instance.baseColliderWidth / 2.0f;
        }
        this.state = State.Init;
    }

    public void InitializeCharacter(int characterId, int characterLevel)
    {
        this.characterInfo = DataMgr.instance.GetCharacterDataElement(characterId);
        this.characterLevel = characterLevel;

        this.characterInfo.AllSkillList.ForEach(skillId => {
            SkillDataElement skillInfo = DataMgr.instance.m_SkillDataElementDic[skillId];
            Skill skill = new Skill(this, skillInfo);
            skillList.Add(skill);
        });
    }

    protected void Start()
    {
        if (this.characterInfo != null)
        {
            this.mspd = characterInfo.MoveSpeed;
            this.basePhysicDefenceStat = characterInfo.PD;
            this.baseMagicDefenceStat = characterInfo.MD;

            this.rangeOfTarget = characterInfo.AttackRange;
            this.attackSpeed =  characterInfo.AttackSpeed;
            this.attackCooltime = this.attackSpeed + Time.fixedDeltaTime;

            normalHitPrefab = ResourceManager.GetHitPrefab("Hit_Base_A");
            if (!string.IsNullOrEmpty(characterInfo.ObjectEffFileName))
            {
                this.projectilePrefab = ResourceManager.GetProjectilePrefab(characterInfo.ObjectEffFileName);
            }
        }

        movementSpeed = GameManager.instance.baseColliderWidth * this.mspd;
        attackStartDistance = GameManager.instance.baseColliderWidth * rangeOfTarget;
        targetStartDistance = attackStartDistance * 2.0f;

        this.animator.SetFloat("attackSpeed", 1.0f / this.attackSpeed);

        hp = MaxHP;

        if (DebugManager.instance.isDebugMode)
        {
            GameObject rangeOfTargetObj = Instantiate(
                DebugManager.instance.circleRangePrefab,
                Vector3.zero,
                Quaternion.identity,
                this.transform
            );
            rangeOfTargetObj.transform.localPosition = Vector3.zero;
            rangeOfTargetObj.transform.localScale = GameManager.instance.baseColliderWidth * rangeOfTarget * 2 * Vector3.one;
            rangeOfTargetObj.GetComponent<SpriteRenderer>().color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.5f);
        }
    }

    public void Move(Vector2 movement)
    {
        Move(movement, movementSpeed);
    }

    protected void Move(Vector2 movement, float speed)
    {
        float absSpeed = Mathf.Max
        (
            Mathf.Abs(movement.x),
            Mathf.Abs(movement.y)
        );
        animator.SetFloat("speed", absSpeed);

        if (Mathf.Abs(movement.x) > 0)
        {
            animator.transform.localScale = new Vector3(
                this.animatorScale.x * -Mathf.Sign(movement.x),
                this.animatorScale.y,
                this.animatorScale.z
            );
        }

        if (movement.x != 0)
        {
            lookingDirection = (movement.x < 0) ? LookDirection.Left : LookDirection.Right;
        }

        this.rigidbody2D.position += new Vector2(
            speed * Time.deltaTime * movement.x,
            speed * Time.deltaTime * movement.y
        );
    }

    protected bool CheckDistanceOver(Vector2 targetPosition, float distance)
    {
        float distanceSqr = Vector2.SqrMagnitude(
            targetPosition - Position2D
        );
        return distanceSqr > distance * distance;
    }

    protected bool CheckDistanceUnder(Vector2 targetPosition, float distance)
    {
        float distanceSqr = Vector2.SqrMagnitude(
            targetPosition - Position2D
        );
        return distanceSqr < distance * distance;
    }

    protected Character GetNearestTarget(Vector2 position)
    {
        Character nearestTarget = null;
        float nearestDistanceSqr = Mathf.Infinity;
        foreach (Character target in GetTargetList(this.characterInfo.DoAerialUnitAttack))
        {
            float distanceSqr = Vector2.SqrMagnitude(position - target.Position2D);
            if (distanceSqr < nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                nearestTarget = target;
            }
        }
        return nearestTarget;
    }
    public List<Character> GetTargetList(bool includeAerial)
    {
        return GetTargetList().FindAll(
            character => includeAerial || character.CharacterInfo.Position == StancePosition_E.Ground
        );
    }
    abstract public List<Character> GetTargetList();
    abstract public List<Character> GetAllyList();

    protected void ChangeState(State newState)
    {
        state = newState;
        curStateTime = 0.0f;

        if (newState == State.Attack)
        {
            OnStartAttack();
        }
    }

    protected virtual void UpdateVariable()
    {

    }

    void UpdateInit()
    {
        if (curStateTime > initTime)
        {
            ChangeState(State.Idle);
        }
    }

    void UpdateIdle()
    {
        if (moveTarget == null)
        {
            moveTarget = GetNearestTarget(Position2D);
        }

        Character nearestTarget = GetNearestTarget(Position2D);
        if (nearestTarget != null)
        {
            if (CheckDistanceUnder(nearestTarget.Position2D, targetStartDistance))
            {
                ChangeState(State.Target);
                target = nearestTarget;
            }

            Vector3 direction = (nearestTarget.Position2D - Position2D).normalized;
            Move(direction);
        }
        else if (moveTarget != null)
        {
            Vector3 direction = (moveTarget.Position2D - Position2D).normalized;
            Move(direction);
        }
        else
        {
            Move(Vector2.zero);
        }
    }

    void UpdateTarget()
    {
        if (target == null || target.IsDead ||
            CheckDistanceOver(target.Position2D, targetStartDistance))
        {
            ChangeState(State.Idle);
            return;
        }

        if (CheckDistanceUnder(target.Position2D, attackStartDistance))
        {
            ChangeState(State.Attack);
            // 바로 공격하게
            curStateTime = float.MaxValue;
        }
        else
        {
            Vector3 direction = (target.Position2D - Position2D).normalized;
            Move(direction);
        }
    }

    void OnStartAttack()
    {
        string triggerName = attackTriggers[Random.Range(0, attackTriggers.Length)];
        animator.SetTrigger(triggerName);
        if (normalSkillPrefab != null)
        {
            GameObject skillObj = Instantiate(
                normalSkillPrefab,
                transform.position,
                Quaternion.identity,
                this.transform
            );
        }
    }

    void UpdateAttack()
    {
        Move(Vector2.zero);
        if (curStateTime > attackCooltime)
        {
            if (target.IsDead)
            {
                target = null;
                ChangeState(State.Idle);
            }
            else if (CheckDistanceOver(target.Position2D, attackStartDistance))
            {
                ChangeState(State.Target);
            }
            else
            {
                ChangeState(State.Attack);
            }
        }
    }

    public void CreateNormalHitObject(Character target)
    {
        if (normalHitPrefab == null) return;

        GameObject.Instantiate(
            normalHitPrefab,
            target.transform.position,
            Quaternion.identity,
            target.transform
        );
    }

    public void Attack()
    {
        if (attackType == AttackType.Melee)
        {
            AttackMelee();
        }
        else
        {
            AttackRange();
        }
    }

    protected void AttackMelee()
    {
        List<Character> targetList = GetTargetList(this.characterInfo.DoAerialUnitAttack);
        foreach (Character target in targetList)
        {
            if (lookingDirection == LookDirection.Left)
            {
                if (target.Position2D.x > Position2D.x)
                {
                    continue;
                }
            }
            else
            {
                if (target.Position2D.x < Position2D.x)
                {
                    continue;
                }
            }
            if (CheckDistanceUnder(target.Position2D, attackStartDistance))
            {
                CreateNormalHitObject(target);
                target.Damage(AttackStat, this.characterInfo.Type);
            }
        }
    }

    protected void AttackRange()
    {
        GameObject projectile = Instantiate(projectilePrefab);
        projectile.transform.position = projectileSpawnPoint.position;
        projectile.transform.rotation = projectileSpawnPoint.rotation;
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        projectileComponent.Initialize(this, target, null);
    }

    protected virtual void OnDamage(float damage) {}

    public void Damage(float attackVal, DamageType_E damageType)
    {
        if (hp <= 0)
        {
            // already die
            return;
        }

        int damage = DataMgr.instance.GetFinalDamage(
            (int)attackVal,
            damageType,
            this.characterInfo.ID,
            1
        );

        OnDamage(damage);

        animator.SetTrigger("DAMAGE");
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    protected void Die()
    {
        collider2d.enabled = false;
        animator.SetBool("DEATH", true);
        ChangeState(State.Dying);
    }

    protected virtual void UpdateManual() {}
    protected virtual void UpdateFollow() {}
    protected virtual void OnDead() {
        gameObject.SetActive(false);
    }

    protected void UpdateDying()
    {
        if (curStateTime > dyingDelay)
        {
            OnDead();
            ChangeState(State.Dead);
        }
    }

    protected void UpdateSkill()
    {
        foreach (Skill skill in skillList)
        {
            skill.UpdateSkill();
        }
    }

    protected void UpdateSkillTimerOnly()
    {
        foreach (Skill skill in skillList)
        {
            skill.UpdateSkillTimer();
        }
    }

    protected void UpdateState()
    {
        curStateTime += Time.deltaTime;

        switch (state)
        {
            case State.Init:
                UpdateInit();
                break;
            case State.Idle:
                UpdateSkill();
                UpdateIdle();
                break;
            case State.Target:
                UpdateSkill();
                UpdateTarget();
                break;
            case State.Attack:
                UpdateSkill();
                UpdateAttack();
                break;
            case State.Manual:
                UpdateSkillTimerOnly();
                UpdateManual();
                break;
            case State.Follow:
                UpdateSkillTimerOnly();
                UpdateFollow();
                break;
            case State.Dying:
                UpdateDying();
                break;
        }
    }

    public void AddSkillEffect(SkillEffect skillEffect)
    {
        if (skillEffect.isOneTimeEffect)
        {
            // Don't Add to List
            skillEffect.ApplyEffect(this);
        }
    }

    protected void Update()
    {
        UpdateVariable();
        UpdateState();
    }

    public void OnLevelUp(int newLevel)
    {
        float prevMaxHP = MaxHP;
        this.characterLevel = newLevel;

        float newMaxHP = MaxHP;

        hp += newMaxHP - prevMaxHP;
    }
}
