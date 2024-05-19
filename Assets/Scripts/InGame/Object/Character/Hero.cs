using System.Collections.Generic;
using UnityEngine;

public class Hero : Character
{
    Player player;

    Vector2 PlayerPosition2D { get { return player.Position2D; } }

    Vector2 followOffset = Vector2.zero;
    float followOffsetRange = 5.0f;
    float followDoneDistance = 5.0f;
    float followSpeed = 20.0f;

    protected override List<CardBuff> GetCardBuffList(CardBuffType_E buffType)
    {
        List<CardBuff> buffList = new ();
        foreach (UserSelectCardDataElement buffCard in GameManager.instance.BuffCardList)
        {
            if (buffCard.TargetSelect == TargetSelect_E.One)
            {
                continue;
            }
            foreach (CardBuff buff in buffCard.CardBuffList)
            {
                if (buff.Type == buffType)
                {
                    buffList.Add(buff);
                }
            }
        }
        return buffList;
    }

    protected override void Awake()
    {
        base.Awake();
        HPBarUI hpBarUI = UIManager.instance.CreateHPBar(this.transform);
        hpBarUI.Initialize(this);

        hpBarUI.GetComponent<SkillBuffUI>().Initialize(this);
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
        
        if (!DebugManager.instance.isAlwaysFollow)
        {
            transform.position = followPosition;
            ChangeState(State.Idle);
        }
        else
        {
            float distanceSqr = (followPosition - Position2D).sqrMagnitude;
            if (distanceSqr < followDoneDistance * followDoneDistance)
            {
                ChangeState(State.Idle);
            }
            else
            {
                Move((followPosition - Position2D).normalized, followSpeed);
            }
        }
    }

    public void FollowPlayer()
    {
        if (state != State.Follow)
        {
            followOffset = Random.insideUnitCircle.normalized * followOffsetRange;
            if (!DebugManager.instance.isAlwaysFollow)
            {
                animator.SetTrigger("CallToArms");
            }
            ChangeState(State.Follow);
        }
    }

    protected override void OnDead()
    {
        base.OnDead();
    }
}
