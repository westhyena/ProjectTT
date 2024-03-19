using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEvent : MonoBehaviour
{
    Character character;
    void Start()
    {
        character = GetComponentInParent<Character>();
    }

    public void OnAttack()
    {
        character.Attack();
    }
}
