using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator animator;
    public float movementSpeed = 20.0f;

    float hp = 100.0f;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Initialize()
    {
        this.hp = 100.0f;
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

        transform.position += new Vector3
        (
            movementSpeed * movement.x * Time.deltaTime,
            movementSpeed * movement.y * Time.deltaTime,
            0.0f
        );
    }
}
