using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hero : Character
{
    Player player;

    Vector2 PlayerPosition2D { get { return player.Position2D; } }

    Vector2 followOffset = Vector2.zero;
    float followOffsetRange = 5.0f;
    float followDoneDistance = 0.1f;
    float followSpeed = 50.0f;

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

    protected override Character GetNearestTarget(Vector2 position)
    {
        return GameManager.instance.GetNearestEnemy(position);
    }

    protected override List<Character> GetTargetList()
    {
        return GameManager.instance.GetEnemyList();
    }

    protected override void UpdateVariable()
    {
        attackCooltime = GameManager.instance.heroAttackCooltime;
        followOffsetRange = GameManager.instance.heroFollowOffsetRange;
        followSpeed = GameManager.instance.heroFollowSpeed;
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
}
