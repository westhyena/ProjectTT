using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Animator animator;
    Player player;

    public float movementSpeed = 20.0f;

    float hp = 100.0f;
    float maxHp = 100.0f;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Initialize(Player player)
    {
        this.player = player;

        this.maxHp = 100.0f;
        this.hp = this.maxHp;
    }

    void Update()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.position += movementSpeed * Time.deltaTime * direction;

        if (Mathf.Abs(direction.x) > 0)
        {
            transform.localScale = new Vector3(
                -Mathf.Sign(direction.x),
                1.0f,
                1.0f
            );
        }
    }
}
