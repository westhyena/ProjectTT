using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float movementSpeed = 20.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(Vector2 movement)
    {
        transform.position += new Vector3(
            movementSpeed * movement.x * Time.deltaTime,
            movementSpeed * movement.y * Time.deltaTime,
            0.0f
        );
    }
}
