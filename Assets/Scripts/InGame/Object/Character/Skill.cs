using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    Character character;
    SkillInfo skillInfo;
    ProjectileInfo projectileInfo;
    GameObject projectilePrefab;

    List<EffectInfo> effectInfoList = new List<EffectInfo>();

    float skillTimer = 0.0f;

    public Skill(Character character, SkillInfo skillInfo)
    {
        this.character = character;
        this.skillInfo = skillInfo;

        AddSkillEffect(skillInfo.skillEffects01);
        AddSkillEffect(skillInfo.skillEffects02);
        AddSkillEffect(skillInfo.skillEffects03);

        if (!string.IsNullOrEmpty(skillInfo.projectileID))
        {
            projectileInfo = DataManager.instance.GetProjectileInfo(skillInfo.projectileID);
            projectilePrefab = ResourceManager.GetProjectilePrefab(projectileInfo.projectilePrefab);
        }
    }

    private void AddSkillEffect(string effectId)
    {
        if (!string.IsNullOrEmpty(effectId))
        {
            effectInfoList.Add(DataManager.instance.GetEffectInfo(effectId));
        }
    }

    Character PickTarget(List<Character> characterList)
    {
        Vector2 position = character.Position2D;
        float range = skillInfo.rangeOfSkill;
        List<Character> insideList = new List<Character>();
        foreach (Character character in characterList)
        {
            float distanceSqr = Vector2.SqrMagnitude(position - character.Position2D);
            if (distanceSqr < range * range)
            {
                insideList.Add(character);
            }
        }

        if (insideList.Count == 0)
        {
            return null;
        }
        return insideList[Random.Range(0, insideList.Count)];
    }

    public void UpdateSkill()
    {
        skillTimer += Time.deltaTime;
        if (skillTimer > skillInfo.coolDown)
        {
            Character target = null;
            if (skillInfo.target == "TOMYSELF")
            {
                target = character;
            }
            else if (skillInfo.target == "TOENEMY")
            {
                target = PickTarget(character.GetTargetList());
            }
            else if (skillInfo.target == "TOALLY")
            {
                target = PickTarget(character.GetAllyList());
            }
            
            if (target != null)
            {
                if (projectilePrefab != null)
                {
                    Projectile projectile = Object.Instantiate(projectilePrefab).GetComponent<Projectile>();
                    projectile.Initialize(character, target, projectileInfo);
                }
                else
                {
                    foreach (EffectInfo effectInfo in effectInfoList)
                    {
                        
                    }
                }
            }
        }

    }
}