using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect
{
    public enum EffectType
    {
        DAMAGE,
        DOT_DAMAGE,
        ATK_UP,
        ATK_SP,
        HEAL,
        IMMUNITY,
        STUN,
        ATK_DOWN
    }
    EffectType effectType;

    float value;
    float duration;
    public bool isOneTimeEffect => duration == 0.0f;
    Character source;
    public SkillEffect(
        EffectType effectType,
        float value,
        float duration,
        Character source
    )
    {
        this.effectType = effectType;
        this.value = value;
        this.duration = duration;
        this.source = source;
    }

    void ApplyDamage(Character target)
    {
        target.Damage(value, null);
    }

    public void ApplyEffect(Character target)
    {
        switch (effectType)
        {
            case EffectType.DAMAGE:
                ApplyDamage(target);
                break;
        }
    }
}