using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public class EffectHolder
    {
        public EffectInfo effectInfo;
        public float value;
        public float duration;
        public EffectHolder(EffectInfo effectInfo, float value, float duration)
        {
            this.effectInfo = effectInfo;
            this.value = value;
            this.duration = duration;
        }
    }
    Character character;
    SkillDataElement skillInfo;
    public SkillDataElement SkillInfo => skillInfo;

    GameObject skillPrefab;
    GameObject hitPrefab;
    GameObject rangeHitPrefab;

    ProjectileInfo projectileInfo;
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


        // AddSkillEffect(skillInfo.skillEffects01, skillInfo.effectsValue01, skillInfo.duration01);
        // AddSkillEffect(skillInfo.skillEffects02, skillInfo.effectsValue02, skillInfo.duration02);
        // AddSkillEffect(skillInfo.skillEffects03, skillInfo.effectsValue03, skillInfo.duration03);

        if (!string.IsNullOrEmpty(skillInfo.ProjectileEffectName))
        {
            // projectileInfo = DataManager.instance.GetProjectileInfo(skillInfo.projectileID);
            // projectilePrefab = ResourceManager.GetProjectilePrefab(projectileInfo.ProjectileEffectName);
        }
    }

    private void AddSkillEffect(string effectId, float value, float duration)
    {
        if (!string.IsNullOrEmpty(effectId))
        {
            effectList.Add(
                new EffectHolder(
                    DataManager.instance.GetEffectInfo(effectId),
                    value,
                    duration
                )
            );
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
            targets = PickTargetInRange(source.GetTargetList(), position2D, range);
        }

        if (targets.Length == 0)
        {
            return targets;
        }

        Character randomTarget = targets[Random.Range(0, targets.Length)];
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
        // foreach (Character effectsTarget in effectsTargets)
        // {
        //     CreateHitObject(effectsTarget);
        //     foreach (EffectHolder holder in effectList)
        //     {
        //         effectsTarget.AddSkillEffect(new SkillEffect(
        //             holder.effectInfo,
        //             holder.value,
        //             holder.duration,
        //             character
        //         ));
        //     }
        // }
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
                    Projectile projectile = Object.Instantiate(projectilePrefab).GetComponent<Projectile>();
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