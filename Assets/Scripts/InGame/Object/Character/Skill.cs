using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    SkillInfo skillInfo;
    public SkillInfo SkillInfo => skillInfo;

    GameObject skillPrefab;
    GameObject hitPrefab;

    ProjectileInfo projectileInfo;
    GameObject projectilePrefab;

    List<EffectHolder> effectList = new ();

    float skillTimer = 0.0f;

    public Skill(Character character, SkillInfo skillInfo)
    {
        this.character = character;
        this.skillInfo = skillInfo;

        if (!string.IsNullOrEmpty(skillInfo.atkAnimation))
        {
            skillPrefab = ResourceManager.GetSkillPrefab(skillInfo.atkAnimation);
        }
        if (!string.IsNullOrEmpty(skillInfo.atkedVFX))
        {
            hitPrefab = ResourceManager.GetHitPrefab(skillInfo.atkedVFX );
        }

        AddSkillEffect(skillInfo.skillEffects01, skillInfo.effectsValue01, skillInfo.duration01);
        AddSkillEffect(skillInfo.skillEffects02, skillInfo.effectsValue02, skillInfo.duration02);
        AddSkillEffect(skillInfo.skillEffects03, skillInfo.effectsValue03, skillInfo.duration03);

        if (!string.IsNullOrEmpty(skillInfo.projectileID))
        {
            projectileInfo = DataManager.instance.GetProjectileInfo(skillInfo.projectileID);
            projectilePrefab = ResourceManager.GetProjectilePrefab(projectileInfo.projectilePrefab);
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

    void CreateHitObject(Character target)
    {
        if (hitPrefab == null) return;

        GameObject.Instantiate(
            hitPrefab,
            target.transform.position,
            Quaternion.identity,
            target.transform
        );
    }

    Character[] GetTarget(
        string target,
        Character source,
        Vector2 position2D,
        float range
    )
    {
        if (target == "ToMySelf")
        {
            return new Character[] { source };
        }
        Character[] targets = new Character[0];
        Debug.Log(target);
        if (target.EndsWith("Ally"))
        {
            targets = PickTargetInRange(source.GetAllyList(), position2D, range);
        }
        else if (target.EndsWith("Enemy") || target.EndsWith("Enemies"))
        {
            targets = PickTargetInRange(source.GetTargetList(), position2D, range);
        }
        Debug.Log("GET TARGET 1" + targets.Length);

        if (target.StartsWith("ToAll"))
        {
            return targets;
        }

        if (targets.Length == 0)
        {
            return targets;
        }

        Character randomTarget = targets[Random.Range(0, targets.Length)];
        return new Character[] { randomTarget };

    }

    public void UpdateSkill()
    {
        skillTimer += Time.deltaTime;
        if (skillTimer > skillInfo.coolDown / 1000.0f)
        {
            Character[] useTargets = GetTarget(
                skillInfo.useTarget,
                character,
                character.Position2D,
                character.RangeOfTarget
            );

            if (useTargets.Length == 0) return;

            skillTimer = 0.0f;
            foreach (Character useTarget in useTargets)
            {
                if (projectilePrefab != null)
                {
                    Projectile projectile = Object.Instantiate(projectilePrefab).GetComponent<Projectile>();
                    projectile.Initialize(character, useTarget, projectileInfo);
                }
                else
                {
                    CreateSkillObject(useTarget);
                    Character[] effectsTargets = GetTarget(
                        skillInfo.effectsTarget,
                        this.character,
                        useTarget.Position2D,
                        skillInfo.rangeOfEffects
                    );
                    Debug.Log($"EFFECT {skillInfo.memo} {effectsTargets.Length}");
                    foreach (Character effectsTarget in effectsTargets)
                    {
                        CreateHitObject(effectsTarget);
                        foreach (EffectHolder holder in effectList)
                        {
                            effectsTarget.AddSkillEffect(new SkillEffect(
                                holder.effectInfo,
                                holder.value,
                                holder.duration,
                                character
                            ));
                        }
                    }
                }
            }
        }
    }
}