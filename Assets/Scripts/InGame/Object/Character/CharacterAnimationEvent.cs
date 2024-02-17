using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEvent : MonoBehaviour
{
    Character character;
    void Awake()
    {
        character = GetComponentInParent<Character>();
    }

    public void OnAttack()
    {
        character.Attack();
    }
}
