using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Vector2 Position2D { get { 
        return new Vector2(
            transform.position.x,
            transform.position.y
        );
    } }

    protected Animator animator;

    float movementSpeed = 20.0f;
    protected void Awake()
    {
        this.animator = GetComponentInChildren<Animator>();
    }

    public void Move(Vector2 movement)
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

    protected bool CheckDistanceOver(Vector2 targetPosition, float distance)
    {
        float distanceSqr = Vector2.SqrMagnitude(
            targetPosition - Position2D
        );
        return distanceSqr > distance * distance;
    }

    protected bool CheckDistanceUnder(Vector2 targetPosition, float distance)
    {
        float distanceSqr = Vector2.SqrMagnitude(
            targetPosition - Position2D
        );
        return distanceSqr < distance * distance;
    }
}
