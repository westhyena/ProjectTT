using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Animator animator;
    Player player;

    public float movementSpeed = 20.0f;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Initialize(Player player)
    {
        this.player = player;
    }

    void Update()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.position += direction * movementSpeed * Time.deltaTime;
    }
}
