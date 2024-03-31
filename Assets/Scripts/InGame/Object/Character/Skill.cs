using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    SkillInfo skillInfo;
    ProjectileInfo projectileInfo;
    GameObject projectilePrefab;

    float skillTimer = 0.0f;

    public Skill(SkillInfo skillInfo)
    {
        this.skillInfo = skillInfo;

        if (!string.IsNullOrEmpty(skillInfo.skillEffects01))
        {
        }

        if (!string.IsNullOrEmpty(skillInfo.projectileID))
        {
            projectileInfo = DataManager.instance.GetProjectileInfo(skillInfo.projectileID);
            projectilePrefab = ResourceManager.GetProjectilePrefab(projectileInfo.projectilePrefab);
        }        
    }

    public void UpdateSkill()
    {
        skillTimer += Time.deltaTime;
        if (skillTimer > skillInfo.coolDown)
        {
        }

    }
}