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

    public override List<Character> GetTargetList()
    {
        return GameManager.instance.GetHeroList(true);
    }

    public override List<Character> GetAllyList()
    {
        return GameManager.instance.GetEnemyList();
    }

    protected override void UpdateVariable()
    {
    }

    protected override void OnDamage(float damage)
    {
        base.OnDamage(damage);

        UIManager.instance.CreateDamageUI(transform.position).Initialize(damage);
    }

    protected override void OnDead()
    {
        base.OnDead();
        
    }
}
