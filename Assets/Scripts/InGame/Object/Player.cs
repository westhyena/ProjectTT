using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public float movementSpeed = 20.0f;

    float hp = 100.0f;
    float maxHp = 100.0f;

    public GameObject attackPrefab;
    public float attackTime = 3.0f;

    public void Initialize()
    {
        this.maxHp = 100.0f;
        this.hp = this.maxHp;
    }
}
