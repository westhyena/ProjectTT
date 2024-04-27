using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect
{
    SkillDataKind_E effectType;

    float value;
    float duration;
    public bool isOneTimeEffect => duration == 0.0f;
    Character source;
    public SkillEffect(
        SkillDataKind_E effectType,
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
        target.Damage(value);
    }

    public void ApplyEffect(Character target)
    {
        switch (effectType)
        {
            case SkillDataKind_E.Damage:
                ApplyDamage(target);
                break;
        }
    }
}