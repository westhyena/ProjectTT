using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hero : MonoBehaviour
{
    Player player;
    Animator animator;
    public float movementSpeed = 21.0f;

    enum HeroState
    {
        Idle,
        Follow,
        Attack
    }
    HeroState state = HeroState.Idle;

    public float followStartDistance = 20.0f;
    public float followEndDistance = 10.0f;
    Vector2 followTargetPosition;

    Vector2 Position2D { get { 
        return new Vector2(
            transform.position.x,
            transform.position.y
        );
    } }
    Vector2 PlayerPosition2D { get { 
        return new Vector2(
            player.transform.position.x,
            player.transform.position.y
        );
    } }

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Initialize(Player player)
    {
        this.player = player;
    }

    void Move(Vector2 movement)
    {
        float absSpeed = Mathf.Max
        (
            Mathf.Abs(movement.x),
            Mathf.Abs(movement.y)
        );
        animator.SetFloat("speed", absSpeed);

        if (Mathf.Abs(movement.x) > 0)
        {
            transform.localScale = new Vector3(
                -Mathf.Sign(movement.x),
                1.0f,
                1.0f
            );
        }

        transform.position += new Vector3(
            movementSpeed * Time.deltaTime * movement.x,
            movementSpeed * Time.deltaTime * movement.y,
            0.0f
        );
    }

    void UpdateIdle()
    {
        float distanceSqr = Vector2.SqrMagnitude(
            Position2D - PlayerPosition2D
        );
        if (distanceSqr > followStartDistance * followStartDistance)
        {
            state = HeroState.Follow;
        }
    }

    void UpdateFollow()
    {
        Vector2 diff = PlayerPosition2D - Position2D;
        Vector2 direction = diff.normalized;

        float distanceSqr = Vector2.SqrMagnitude(diff);
        if (distanceSqr < followEndDistance * followEndDistance)
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
