using System.Collections.Generic;
using UnityEngine;

public class Hero : Character
{
    Player player;

    Vector2 PlayerPosition2D { get { return player.Position2D; } }

    Vector2 followOffset = Vector2.zero;
    float followOffsetRange = 5.0f;

    protected override void Awake()
    {
        base.Awake();
        HPBarUI hpBarUI = UIManager.instance.CreateHPBar(this.transform);
        hpBarUI.Initialize(this);
    }

    public void Initialize(Player player)
    {
        this.player = player;
        maxHp = 100.0f;
        hp = maxHp;
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
        followOffsetRange = GameManager.instance.heroFollowOffsetRange;
        this.characterLevel = GameManager.instance.GetCompanionLevel(this.CharacterInfo.ID);
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
