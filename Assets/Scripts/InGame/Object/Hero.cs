using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hero : Character
{
    Player player;

    Vector2 PlayerPosition2D { get { return player.Position2D; } }

    public void Initialize(Player player)
    {
        this.player = player;
    }

    protected override Character GetNearestTarget(Vector2 position)
    {
        return GameManager.instance.GetNearestEnemy(position);
    }

    protected override void UpdateVariable()
    {
        movementSpeed = GameManager.instance.heroMovementSpeed;
        targetStartDistance = GameManager.instance.heroTargetStartDistance;
    }
}
