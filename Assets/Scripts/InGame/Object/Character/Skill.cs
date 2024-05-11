using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public class EffectHolder
    {
        public SkillEffect effectInfo;
        float timer = 0.0f;
        float dotTimer = 0.0f;
        float CONST_DOT_TIME = 1.0f;
        public bool isEnd => timer > effectInfo.Duration;

        public EffectHolder(SkillEffect effectInfo)
        {
            this.effectInfo = effectInfo;
        }

        public void Update(Character target)
        {
            timer += Time.deltaTime;
            if (effectInfo.IsDotEffect)
            {
                UpdateDot(target);
            }
        }

        void UpdateDot(Character target)
        {
            dotTimer += Time.deltaTime;
            if (dotTimer > CONST_DOT_TIME)
            {
                effectInfo.ApplyEffect(target);
                dotTimer = 0.0f;
            }
        }
    }

    Character character;
    SkillDataElement skillInfo;
    public SkillDataElement SkillInfo => skillInfo;

    GameObject skillPrefab;
    GameObject hitPrefab;
    GameObject rangeHitPrefab;

    GameObject projectilePrefab;

    List<EffectHolder> effectList = new ();

    float skillTimer = 0.0f;

    public Skill(Character character, SkillDataElement skillInfo)
    {
        this.character = character;
        this.skillInfo = skillInfo;

        if (!string.IsNullOrEmpty(skillInfo.MyPositionEffectName))
        {
            skillPrefab = ResourceManager.GetSkillPrefab(skillInfo.MyPositionEffectName);
        }
        if (!string.IsNullOrEmpty(skillInfo.ObjectDamageEffectName))
        {
            hitPrefab = ResourceManager.GetHitPrefab(skillInfo.ObjectDamageEffectName);
        }
        if (!string.IsNullOrEmpty(skillInfo.TargetPointEffectName))
        {
            rangeHitPrefab = ResourceManager.GetHitPrefab(skillInfo.TargetPointEffectName);
        }

        foreach (SkillDataBase skillEffect in skillInfo.SkillData)
        {
            effectList.Add(new EffectHolder(new SkillEffect(
                skillEffect.SkillDataKind,
                skillInfo.Type,
                skillEffect.Value,
                skillEffect.Time,
                this.character
            )));
        }

        if (!string.IsNullOrEmpty(skillInfo.ProjectileEffectName))
        {
            projectilePrefab = ResourceManager.GetProjectilePrefab(skillInfo.ProjectileEffectName);
        }
    }

    Character[] PickTargetInRange(List<Character> characterList, Vector2 position, float range)
    {
        float targetDistance = range * GameManager.instance.baseColliderWidth;
        List<Character> insideList = new ();
        foreach (Character target in characterList)
        {
            float distanceSqr = Vector2.SqrMagnitude(position - target.Position2D);
            if (distanceSqr < targetDistance * targetDistance)
            {
                insideList.Add(target);
            }
        }

        return insideList.ToArray();
    }

    void CreateSkillObject(Character target)
    {
        if (skillPrefab == null) return;

        GameObject.Instantiate(
            skillPrefab,
            target.transform.position,
            Quaternion.identity,
            target.transform
        );
    }

    public void CreateHitObject(Character target)
    {
        if (hitPrefab == null) return;

        GameObject.Instantiate(
            hitPrefab,
            target.transform.position,
            Quaternion.identity,
            target.transform
        );
    }

    public void CreateRangeHitObject(Vector2 targetPosition)
    {
        if (rangeHitPrefab == null) return;

        GameObject.Instantiate(
            rangeHitPrefab,
            (Vector3)targetPosition,
            Quaternion.identity
        );
    }

    Character[] GetTarget(
        ActivePosition_E activePosition,
        Target_E target,
        Character source,
        Vector2 position2D,
        float range
    )
    {
        if (activePosition == ActivePosition_E.Me)
        {
            return new Character[] { source };
        }
        Character[] targets = new Character[0];
        if (target == Target_E.Our)
        {
            targets = PickTargetInRange(source.GetAllyList(), position2D, range);
        }
        else if (target == Target_E.Enemy)
        {
            targets = PickTargetInRange(
                source.GetTargetList(this.skillInfo.DoAerialUnitAttack),
                position2D,
                range
            );
        }

        if (targets.Length == 0)
        {
            return targets;
        }

        Character randomTarget = targets[UnityEngine.Random.Range(0, targets.Length)];
        return new Character[] { randomTarget };
    }

    public void UseSkillOnTarget(Character useTarget)
    {
        CreateSkillObject(useTarget);
        Character[] effectsTargets = GetTarget(
            skillInfo.ActivePosition,
            skillInfo.Target,
            this.character,
            useTarget.Position2D,
            skillInfo.DamageTypeRange
        );
        foreach (Character effectsTarget in effectsTargets)
        {
            CreateHitObject(effectsTarget);
            foreach (EffectHolder effect in effectList)
            {
                effectsTarget.AddSkillEffect(effect);
            }
        }
    }

    public void UpdateSkillTimer()
    {
        skillTimer += Time.deltaTime;
    }

    public void UpdateSkill()
    {
        UpdateSkillTimer();
        if (skillTimer > skillInfo.CoolTime)
        {
            Character[] useTargets = GetTarget(
                skillInfo.ActivePosition,
                skillInfo.Target,
                this.character,
                this.character.Position2D,
                skillInfo.DamageTypeRange
            );

            if (useTargets.Length == 0) return;

            skillTimer = 0.0f;
            foreach (Character useTarget in useTargets)
            {
                if (projectilePrefab != null)
                {
                    Projectile projectile = GameObject.Instantiate(projectilePrefab).GetComponent<Projectile>();
                    projectile.Initialize(character, useTarget, this);
                }
                else
                {
                    UseSkillOnTarget(useTarget);
                }
            }
        }
    }
}