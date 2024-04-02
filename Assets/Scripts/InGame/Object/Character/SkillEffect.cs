using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect
{
    EffectInfo effectInfo;
    public string Memo => effectInfo.memo;
    float value;
    float duration;
    public bool isOneTimeEffect => duration == 0.0f;
    Character source;
    public SkillEffect(
        EffectInfo effectInfo,
        float value,
        float duration,
        Character source
    )
    {
        this.effectInfo = effectInfo;
        this.value = value;
        this.duration = duration;
        this.source = source;
    }

    void ApplyDamage(Character target)
    {
        float damage = 0.0f;
        if (effectInfo.basedStat == "ATTACK")
        {
            damage = source.AttackStat * value / 10000.0f;
        }

        target.Damage(damage, null);
    }

    public void ApplyEffect(Character target)
    {
        switch (effectInfo.effectType)
        {
            case "DAMAGE":
                ApplyDamage(target);
                break;
        }
    }
}