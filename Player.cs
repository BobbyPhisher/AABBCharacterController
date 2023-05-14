using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    private float moveSpeed = 2;
    private float jumpVelocity = 6;
    private float gravity = -9;

    public Vector3 velocity;
    Controller2D controller;

    void Start()
    {
        controller = GetComponent<Controller2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Reset the velocity here if there are collisions below or above us
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        // Player Movement
        Vector2 input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

        // Jump
        if (Input.GetKeyDown(KeyCode.W) && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        // Apply forces to our player's position
        velocity.x = input.x * moveSpeed;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
