using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    Vector2 movement;

    HeroManager heroManager;

    public FloatingJoystick joystick;
    
    void Awake()
    {
        heroManager = GetComponent<HeroManager>();
        joystick = FindObjectOfType<FloatingJoystick>();
    }

    void Update()
    {
        Player player = GameManager.instance.Player;

        movement = Vector2.zero;
        if (joystick != null)
        {
            movement = joystick.Direction;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            movement.y = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            movement.y = -1;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement.x = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            movement.x = 1;
        }

        player.ManualMove(movement);

        if (Input.GetKeyDown(KeyCode.C))
        {
            GameManager.instance.SummonCompanion();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            GameManager.instance.CallCompanion();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (Enemy enemy in EnemyManager.instance.AliveEnemyList)
            {
                enemy.Damage(999999999, DamageType_E.Physics);
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale > 0.0f)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
    }
}
