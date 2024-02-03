using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    Player player;

    float hp = 100.0f;
    float maxHp = 100.0f;

    public void Initialize(Player player)
    {
        this.player = player;

        this.maxHp = 100.0f;
        this.hp = this.maxHp;
    }

    protected override Character GetNearestTarget(Vector2 position)
    {
        return GameManager.instance.GetNearestHero(position);
    }

    protected override void UpdateVariable()
    {
        movementSpeed = GameManager.instance.enemyMovementSpeed;
        targetStartDistance = GameManager.instance.enemyTargetStartDistance;
        attackStartDistance = GameManager.instance.enemyAttackStartDistance;
        attackCooltime = GameManager.instance.enemyAttackCooltime;

    }
}
