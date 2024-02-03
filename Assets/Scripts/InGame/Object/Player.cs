using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Player : Character
{
    float hp = 100.0f;
    float maxHp = 100.0f;

    public float manualEndTime = 1.0f;

    public void Initialize()
    {
        this.maxHp = 100.0f;
        this.hp = this.maxHp;
    }

    protected override Character GetNearestTarget(Vector2 position)
    {
        return GameManager.instance.GetNearestEnemy(position);
    }

    protected override void UpdateVariable()
    {
        movementSpeed = GameManager.instance.playerMovementSpeed;
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
        if (movement.sqrMagnitude > 0)
        {
            ChangeState(State.Manual);
            Move(movement);
        }
    }
}
