using System.Collections.Generic;
using UnityEngine;

public class Hero : Character
{
    Player player;

    Vector2 PlayerPosition2D { get { return player.Position2D; } }

    Vector2 followOffset = Vector2.zero;
    float followOffsetRange = 5.0f;

    public override float AttackStat
    {
        get
        {
            float baseStat = base.AttackStat;
            float ratio = 1.0f;
            foreach (UserSelectCardDataElement buffCard in GameManager.instance.BuffCardList)
            {
                if (buffCard.TargetSelect == TargetSelect_E.One)
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
                if (buffCard.TargetSelect == TargetSelect_E.One)
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
        HPBarUI hpBarUI = UIManager.instance.CreateHPBar(this.transform);
        hpBarUI.Initialize(this);
    }

    public void Initialize(Player player)
    {
        this.player = player;
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
        followOffsetRange = GameManager.instance.heroFollowOffsetRange;
    }

    protected override void UpdateFollow()
    {
        base.UpdateFollow();
        Vector2 followPosition = PlayerPosition2D + followOffset;
        transform.position = followPosition;
        ChangeState(State.Idle);

        // float distanceSqr = (followPosition - Position2D).sqrMagnitude;
        // if (distanceSqr < followDoneDistance * followDoneDistance)
        // {
        //     ChangeState(State.Idle);
        // }
        // else
        // {
        //     Move((followPosition - Position2D).normalized, followSpeed);
        // }
    }

    public void FollowPlayer()
    {
        followOffset = Random.insideUnitCircle.normalized * followOffsetRange;
        animator.SetTrigger("CallToArms");
        ChangeState(State.Follow);
    }

    protected override void OnDead()
    {
        base.OnDead();
    }
}
