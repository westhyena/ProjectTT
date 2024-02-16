using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Player : Character
{
    public float manualEndTime = 1.0f;
    Vector2 manualMovement = Vector2.zero;

    public void Initialize()
    {
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
        attackDamage = GameManager.instance.playerAttackDamage;
        attackCooltime = GameManager.instance.playerAttackCooltime;
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
        Move(movement);
        if (movement.sqrMagnitude > 0)
        {
            ChangeState(State.Manual);
        }
    }
}
