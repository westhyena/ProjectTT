using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hero : Character
{
    Player player;
    public float movementSpeed = 21.0f;

    enum HeroState
    {
        Idle,
        Follow,
        Target,
        Attack
    }
    HeroState state = HeroState.Idle;
    float curStateTime = 0.0f;

    float followStartDistance = 20.0f;
    float followEndDistance = 10.0f;
    Vector2 followTargetPosition;

    float targetStartDistance = 20.0f;
    Enemy targetEnemy = null;

    float attackStartRange = 5.0f;
    float attackCooltime = 2.0f;

    Vector2 PlayerPosition2D { get { return player.Position2D; } }

    public void Initialize(Player player)
    {
        this.player = player;
    }

    void ChangeState(HeroState newState)
    {
        state = newState;
        curStateTime = 0.0f;
    }

    void UpdateIdle()
    {
        Enemy nearestEnemy = GameManager.instance.GetNearestEnemy(Position2D);
        if (CheckDistanceOver(PlayerPosition2D, followStartDistance))
        {
            ChangeState(HeroState.Follow);
        }
        else if (nearestEnemy != null)
        {
            if (CheckDistanceUnder(nearestEnemy.Position2D, targetStartDistance))
            {
                ChangeState(HeroState.Target);
                targetEnemy = nearestEnemy;
            }
        }
    }

    void UpdateFollow()
    {
        Vector2 diff = PlayerPosition2D - Position2D;
        Vector2 direction = diff.normalized;

        if (CheckDistanceUnder(PlayerPosition2D, followEndDistance))
        {
            ChangeState(HeroState.Idle);
        }
        else
        {
            Move(direction);
        }
    }

    void UpdateTarget()
    {
        if (CheckDistanceOver(PlayerPosition2D, followStartDistance))
        {
            ChangeState(HeroState.Follow);
            return;
        }

        Enemy nearestEnemy = GameManager.instance.GetNearestEnemy(Position2D);
        if (nearestEnemy != targetEnemy)
        {
            targetEnemy = nearestEnemy;
        }

        if (CheckDistanceOver(targetEnemy.Position2D, targetStartDistance))
        {
            ChangeState(HeroState.Idle);
        }
        else if (CheckDistanceUnder(targetEnemy.Position2D, attackStartRange))
        {
            animator.SetTrigger("attack");
            ChangeState(HeroState.Attack);
        }
        else
        {
            Vector2 diff = targetEnemy.Position2D - Position2D;
            Vector2 direction = diff.normalized;
            Move(direction);
        }
    }

    void UpdateAttack()
    {
        if (curStateTime > attackCooltime)
        {
            animator.SetTrigger("attack");
            ChangeState(HeroState.Attack);
        }
        else if (CheckDistanceOver(PlayerPosition2D, followStartDistance))
        {
            ChangeState(HeroState.Follow);
            return;
        }
        else if (CheckDistanceOver(targetEnemy.Position2D, attackStartRange))
        {
            ChangeState(HeroState.Target);
        }
    }

    void Update()
    {
        curStateTime += Time.deltaTime;
        switch (state)
        {
            case HeroState.Idle:
                UpdateIdle();
                break;
            case HeroState.Follow:
                UpdateFollow();
                break;
            case HeroState.Target:
                UpdateTarget();
                break;
            case HeroState.Attack:
                UpdateAttack();
                break;

        }
    }
}
