using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    Vector2 movement;

    public Player player;

    void Update()
    {
        movement = Vector2.zero;
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

        player.Move(movement);
    }
}
