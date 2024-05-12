using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect
{
    SkillDataKind_E effectType;
    public SkillDataKind_E EffectType => effectType;
    DamageType_E damageType;
    public DamageType_E DamageType => damageType;

    float value;
    public float Value => value;
    float duration;
    public float Duration => duration;
    public bool IsOneTimeEffect => duration == 0.0f;
    public bool IsDotEffect => effectType == SkillDataKind_E.DotDamage;
    Character source;
    public SkillEffect(
        SkillDataKind_E effectType,
        DamageType_E damageType,
        float value,
        float duration,
        Character source
    )
    {
        this.effectType = effectType;
        this.damageType = damageType;
        this.value = value;
        this.duration = duration;
        this.source = source;
    }

    void ApplyDamage(Character target)
    {
        target.Damage(value, damageType);
    }

    void ApplyHeal(Character target)
    {
        target.Heal(value);
    }

    public void ApplyEffect(Character target)
    {
        switch (effectType)
        {
            case SkillDataKind_E.Damage:
            case SkillDataKind_E.DotDamage:
                ApplyDamage(target);
                break;
            case SkillDataKind_E.Heal:
                ApplyHeal(target);
                break;
        }
    }
}