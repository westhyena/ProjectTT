using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    float manualEndTime = 0.0f;
    private Vector2 manualMovement = Vector2.zero;
    GameObject followEffect;

    protected override List<CardBuff> GetCardBuffList(CardBuffType_E buffType)
    {
        List<CardBuff> buffList = new ();
        foreach (UserSelectCardDataElement buffCard in GameManager.instance.BuffCardList)
        {
            if (buffCard.TargetSelect == TargetSelect_E.Area)
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
        followEffect = Instantiate(EffectManager.instance.followEffectPrefab, transform);
        followEffect.SetActive(false);

        HPBarUI hpBarUI = UIManager.instance.CreatePlayerHPBar(this.transform);
        hpBarUI.Initialize(this);

        hpBarUI.GetComponent<SkillBuffUI>().Initialize(this);
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

            if (DebugManager.instance.isAlwaysFollow)
            {
                HeroManager.instance.AliveHeroList.ForEach(hero => hero.FollowPlayer());
            }
        }
    }

    public void OnFollowCall()
    {
        followEffect.SetActive(false);
        followEffect.SetActive(true);
    }

    protected override void UpdateIdle()
    {
        if (DebugManager.instance.isPlayerAuto)
        {
            base.UpdateIdle();
        }
        else
        {
            Character nearestTarget = GetNearestTarget(Position2D);
            if (nearestTarget != null)
            {
                if (CheckDistanceUnder(nearestTarget.Position2D, targetStartDistance))
                {
                    ChangeState(State.Target);
                    target = nearestTarget;
                }
            }

            Move(Vector2.zero);
        }
    }

    protected override void UpdateTarget()
    {
        if (DebugManager.instance.isPlayerAuto)
        {
            base.UpdateTarget();
        }
        else
        {
            if (target == null || target.IsDead ||
                CheckDistanceOver(target.Position2D, targetStartDistance))
            {
                ChangeState(State.Idle);
                return;
            }

            if (CheckDistanceUnder(target.Position2D, attackStartDistance))
            {
                ChangeState(State.Attack);
                // 바로 공격하게
                curStateTime = float.MaxValue;
            }

            Move(Vector2.zero);
        }
    }
}
