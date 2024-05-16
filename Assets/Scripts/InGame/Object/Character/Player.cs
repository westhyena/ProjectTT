using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    float manualEndTime = 0.0f;
    private Vector2 manualMovement = Vector2.zero;
    GameObject followEffect;

    public override float AttackStat
    {
        get
        {
            float baseStat = base.AttackStat;
            float ratio = 1.0f;
            foreach (UserSelectCardDataElement buffCard in GameManager.instance.BuffCardList)
            {
                if (buffCard.TargetSelect == TargetSelect_E.Area)
                {
                    continue;
                }
                foreach (CardBuff buff in buffCard.CardBuffList)
                {
                    if (buff.Type == CardBuffType_E.AttackUp_Percent)
                    {
                        ratio += buff.Value / 100.0f;
                    }
                }
            }
            return baseStat * ratio;
        }
    }

    public override float AttackSpeed
    {
        get
        {
            float ratio = 1.0f;
            foreach (Skill.EffectHolder effectHolder in this.skillEffectList)
            {
                if (effectHolder.effectInfo.EffectType == SkillDataKind_E.AttackSpeedUp)
                {
                    ratio += effectHolder.effectInfo.Value;
                }
                else if (effectHolder.effectInfo.EffectType == SkillDataKind_E.AttackSpeedDown)
                {
                    ratio -= effectHolder.effectInfo.Value;
                }
            }
            foreach (UserSelectCardDataElement buffCard in GameManager.instance.BuffCardList)
            {
                if (buffCard.TargetSelect == TargetSelect_E.Area)
                {
                    continue;
                }
                foreach (CardBuff buff in buffCard.CardBuffList)
                {
                    if (buff.Type == CardBuffType_E.AttackSpeed_Percent)
                    {
                        ratio -= buff.Value / 100.0f;
                    }
                }
            }
            return baseAttackSpeed * ratio;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        followEffect = Instantiate(EffectManager.instance.followEffectPrefab, transform);
        followEffect.SetActive(false);

        HPBarUI hpBarUI = UIManager.instance.CreatePlayerHPBar(this.transform);
        hpBarUI.Initialize(this);
    }

    public override List<Character> GetTargetList()
    {
        return GameManager.instance.GetEnemyList();
    }

    public override List<Character> GetAllyList()
    {
        return GameManager.instance.GetHeroList(true);
    }

    protected override void UpdateVariable()
    {
        base.UpdateVariable();
    }

    protected override void UpdateManual()
    {
        base.UpdateManual();

        if (curStateTime > manualEndTime)
        {
            ChangeState(State.Idle);
        }
    }

    public void ManualMove(Vector2 movement)
    {
        if (IsStun)
        {
            return;
        }
        if (movement.sqrMagnitude > 0)
        {
            ChangeState(State.Manual);
            Move(movement);
        }
    }

    public void OnFollowCall()
    {
        followEffect.SetActive(false);
        followEffect.SetActive(true);
    }
}
