using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    Player player;

    public float movementSpeed = 20.0f;

    enum EnemyState
    {
        Idle,
        Target,
        Attack
    }
    EnemyState state = EnemyState.Idle;
    float curStateTime = 0.0f;

    float targetStartDistance = 20.0f;
    Hero targetHero = null;

    float attackStartRange = 5.0f;
    float attackCooltime = 3.0f;

    float hp = 100.0f;
    float maxHp = 100.0f;

    public void Initialize(Player player)
    {
        this.player = player;

        this.maxHp = 100.0f;
        this.hp = this.maxHp;
    }

    void ChangeState(EnemyState newState)
    {
        state = newState;
        curStateTime = 0.0f;
    }

    void UpdateIdle()
    {
        Hero nerarestHero = GameManager.instance.GetNearestHero(Position2D);
        if (nerarestHero != null)
        {
            if (CheckDistanceUnder(nerarestHero.Position2D, targetStartDistance))
            {
                ChangeState(EnemyState.Target);
                targetHero = nerarestHero;
            }
        }
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Move(direction);
    }

    void UpdateTarget()
    {
        if (targetHero == null)
        {
            ChangeState(EnemyState.Idle);
            return;
        }

        Hero nearestHero = GameManager.instance.GetNearestHero(Position2D);
        if (nearestHero != targetHero)
        {
            targetHero = nearestHero;
        }

        if (CheckDistanceOver(targetHero.Position2D, targetStartDistance))
        {
            ChangeState(EnemyState.Idle);
        }
        else if (CheckDistanceUnder(targetHero.Position2D, attackStartRange))
        {
            animator.SetTrigger("attack");
            ChangeState(EnemyState.Attack);
        }
        else
        {
            Vector2 diff = targetHero.Position2D - Position2D;
            Vector2 direction = diff.normalized;
            Move(direction);
        }
    }

    void UpdateAttack()
    {
        if (curStateTime > attackCooltime)
        {
            animator.SetTrigger("attack");
            ChangeState(EnemyState.Attack);
        }
        if (CheckDistanceOver(targetHero.Position2D, attackStartRange))
        {
            ChangeState(EnemyState.Target);
        }
    }

    void Update()
    {
        curStateTime += Time.deltaTime;
        switch (state)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;
            case EnemyState.Target:
                UpdateTarget();
                break;
            case EnemyState.Attack:
                UpdateAttack();
                break;
        }
    }
}
