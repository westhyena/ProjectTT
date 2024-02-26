using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    Vector2 movement;

    public Player player;

    HeroManager heroManager;

    public FloatingJoystick joystick;
    
    void Awake()
    {
        heroManager = GetComponent<HeroManager>();
        joystick = FindObjectOfType<FloatingJoystick>();
    }

    void Update()
    {
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
            heroManager.CreateHero();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            player.OnFollowCall();
            foreach (Hero hero in heroManager.HeroList)
            {
                hero.FollowPlayer();
            }
        }
    }
}
