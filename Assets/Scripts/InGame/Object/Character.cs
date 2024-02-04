using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public enum State
    {
        Idle,  // 기본 상태.
        Move,  // 적을 찾아 이동 중
        Target,  // 적이 타겟된 상태
        Attack,
        Dying,

        Manual,
        Follow,
    }

    public enum LookDirection
    {
        Left,
        Right
    }

    protected State state = State.Idle;
    protected LookDirection lookingDirection = LookDirection.Left;
    protected float curStateTime = 0.0f;

    public Vector2 Position2D { get { 
        return new Vector2(
            transform.position.x,
            transform.position.y
        );
    } }

    protected Animator animator;
    protected Collider2D collider2d;

    protected float movementSpeed = 20.0f;

    protected Character moveTarget = null;
    protected float targetStartDistance = 20.0f;
    protected float attackStartDistance = 5.0f;
    protected float attackCooltime = 3.0f;
    protected bool attackPerformed = false;
    protected float attackDelay = 0.2f;

    protected Character target = null;

    protected float hp = 100.0f;
    protected float maxHp = 100.0f;
    public bool IsDead { get { return hp <= 0.0f; } }

    protected float attackDamage = 10.0f;

    protected void Awake()
    {
        this.animator = GetComponentInChildren<Animator>();
        this.collider2d = GetComponent<Collider2D>();
        this.state = State.Idle;
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
            transform.localScale = new Vector3(
                -Mathf.Sign(movement.x),
                1.0f,
                1.0f
            );
        }

        lookingDirection = (movement.x < 0) ? LookDirection.Left : LookDirection.Right;

        transform.position += new Vector3(
            speed * Time.deltaTime * movement.x,
            speed * Time.deltaTime * movement.y,
            0.0f
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

    abstract protected Character GetNearestTarget(Vector2 position);
    abstract protected List<Character> GetTargetList();

    protected void ChangeState(State newState)
    {
        state = newState;
        curStateTime = 0.0f;

        if (newState == State.Attack)
        {
            attackPerformed = false;
        }
    }

    protected virtual void UpdateVariable()
    {

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
    }

    void UpdateTarget()
    {
        if (target == null || target.IsDead)
        {
            ChangeState(State.Idle);
            return;
        }

        if (CheckDistanceUnder(target.Position2D, attackStartDistance))
        {
            animator.SetTrigger("attack");
            ChangeState(State.Attack);
        }
        else
        {
            Vector3 direction = (target.Position2D - Position2D).normalized;
            Move(direction);
        }
    }

    void UpdateAttack()
    {
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
                animator.SetTrigger("attack");
                ChangeState(State.Attack);
            }
        }
        else if (!attackPerformed && curStateTime > attackDelay)
        {
            Attack();
            attackPerformed = true;
        }
    }

    void Attack()
    {
        AttackInRange();
    }

    void AttackInRange()
    {
        List<Character> targetList = GetTargetList();
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
                target.Damage(attackDamage);
            }
        }
    }

    public void Damage(float damage)
    {
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

    protected void UpdateState()
    {
        curStateTime += Time.deltaTime;

        switch  (state)
        {
            case State.Idle:
                UpdateIdle();
                break;
            case State.Target:
                UpdateTarget();
                break;
            case State.Attack:
                UpdateAttack();
                break;
            case State.Manual:
                UpdateManual();
                break;
            case State.Follow:
                UpdateFollow();
                break;
        }
    }

    protected void Update()
    {
        UpdateVariable();
        UpdateState();
    }
}
