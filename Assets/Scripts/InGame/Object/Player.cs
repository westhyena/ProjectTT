using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator animator;
    public float movementSpeed = 20.0f;

    new Rigidbody2D rigidbody;

    float hp = 100.0f;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
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

        //rigidbody.MovePosition(movementSpeed * Time.deltaTime * movement);
        transform.position += new Vector3(
            movementSpeed * Time.deltaTime * movement.x,
            movementSpeed * Time.deltaTime * movement.y,
            0.0f
        );
    }
}
