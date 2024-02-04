using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    Player player;

    public void Initialize(Player player)
    {
        this.player = player;

        maxHp = 100.0f;
        hp = maxHp;
    }

    protected override Character GetNearestTarget(Vector2 position)
    {
        return GameManager.instance.GetNearestHero(position);
    }

    protected override List<Character> GetTargetList()
    {
        return GameManager.instance.GetHeroList(true);
    }

    protected override void UpdateVariable()
    {
        movementSpeed = GameManager.instance.enemyMovementSpeed;
        targetStartDistance = GameManager.instance.enemyTargetStartDistance;
        attackStartDistance = GameManager.instance.enemyAttackStartDistance;
        attackCooltime = GameManager.instance.enemyAttackCooltime;
        attackDamage = GameManager.instance.enemyAttackDamage;
    }
}
