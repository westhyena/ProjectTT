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
        Attack
    }
    HeroState state = HeroState.Idle;

    float followStartDistance = 20.0f;
    float followEndDistance = 10.0f;
    Vector2 followTargetPosition;

    Vector2 PlayerPosition2D { get { return player.Position2D; } }

    public void Initialize(Player player)
    {
        this.player = player;
    }

    void UpdateIdle()
    {
        if (CheckDistanceOver(PlayerPosition2D, followStartDistance))
        {
            state = HeroState.Follow;
        }
    }

    void UpdateFollow()
    {
        Vector2 diff = PlayerPosition2D - Position2D;
        Vector2 direction = diff.normalized;

        if (CheckDistanceUnder(PlayerPosition2D, followEndDistance))
        {
            state = HeroState.Idle;
        }
        else
        {
            Move(direction);
        }
    }

    void Update()
    {
        switch (state)
        {
            case HeroState.Idle:
                UpdateIdle();
                break;
            case HeroState.Follow:
                UpdateFollow();
                break;

        }
    }
}
