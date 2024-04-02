using System.Collections;
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
        if (!string.IsNullOrEmpty(skillInfo.atkedEffect))
        {
            hitPrefab = ResourceManager.GetHitPrefab(skillInfo.atkedEffect);
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

    Character[] PickTargetInRange(List<Character> characterList)
    {
        Vector2 position = character.Position2D;
        float range = skillInfo.rangeOfSkill * GameManager.instance.baseColliderWidth;
        List<Character> insideList = new ();
        foreach (Character target in characterList)
        {
            float distanceSqr = Vector2.SqrMagnitude(position - target.Position2D);
            if (distanceSqr < range * range)
            {
                insideList.Add(target);
            }
        }

        return insideList.ToArray();
    }

    void CreateSkillObject()
    {
        if (skillPrefab == null) return;

        GameObject.Instantiate(
            skillPrefab,
            character.transform.position,
            Quaternion.identity,
            character.transform
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

    public void UpdateSkill()
    {
        skillTimer += Time.deltaTime;
        if (skillTimer > skillInfo.coolDown / 1000.0f)
        {
            Character[] targets = new Character[0];
            if (skillInfo.target == "TOMYSELF")
            {
                targets = new Character[] { character };
            }
            else if (skillInfo.target == "TOENEMY")
            {
                targets = PickTargetInRange(character.GetTargetList());
            }
            else if (skillInfo.target == "TOALLY")
            {
                targets = PickTargetInRange(character.GetAllyList());
            }

            if (targets.Length > 0)
            {
                if (!skillInfo.isRangeAttack)
                {
                    Character randomTarget = targets[Random.Range(0, targets.Length)];
                    targets = new Character[] { randomTarget };
                }

                skillTimer = 0.0f;
                CreateSkillObject();

                foreach (Character target in targets)
                {
                    if (projectilePrefab != null)
                    {
                        Projectile projectile = Object.Instantiate(projectilePrefab).GetComponent<Projectile>();
                        projectile.Initialize(character, target, projectileInfo);
                    }
                    else
                    {
                        CreateHitObject(target);
                        foreach (EffectHolder holder in effectList)
                        {
                            target.AddSkillEffect(new SkillEffect(
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