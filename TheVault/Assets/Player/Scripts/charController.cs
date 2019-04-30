﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charController : MonoBehaviour
{

    [SerializeField]
    float moveSpeed = 4f;

    public Transform playerTransform;
    public GameObject gameController;
    public float maxDashDistance;
    public float dashRate;
    public float playerCurrentHealth;
    Vector3 forward, right;

    private float nextDash;

    RaycastHit hit;


    // Start is called before the first frame update
    void Start()
    {
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("HorizontalKey") || Input.GetButton("VerticalKey"))
        {
            // Activate move method when using "w" "a" "s" or "d"
            Move();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > nextDash)
        {
            // Check dash cooldown when pressing left shfit
            // Activate dash method 
            nextDash = Time.time + dashRate;
            Dash();
        }

    }

    void Move()
    {
        // finds what directions are being inputted
        Vector3 direction = new Vector3(Input.GetAxis("HorizontalKey"), 0, Input.GetAxis("VerticalKey"));
        
        // finds horizonal speed
        Vector3 rightMovement = right * moveSpeed * Time.deltaTime * Input.GetAxis("HorizontalKey");
        
        // finds vertical speed
        Vector3 upMovement = forward * moveSpeed * Time.deltaTime * Input.GetAxis("VerticalKey");

        // finds the direction to face
        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        // changes players facing direction
        transform.forward = heading;

        // changes player horizontal position 
        transform.position += rightMovement;

        // changes player vertical position
        transform.position += upMovement;
    }
    void Dash()
    {
        float dashDistance = maxDashDistance;

        // uses raycast from player position, out from the direction the player is facing, to the distance equal to the player's maximum dash distance
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDashDistance))
        {
            // if the ray cast hits something, the player's dash distance is reduced
            // prevents the player from teleporting through walls
            dashDistance = hit.distance;
        }

        // teleports the player
        playerTransform.transform.position += playerTransform.transform.forward * dashDistance;
    }
    public void TakeDamage(float amount)
    {
        // Reduce current health by the amount of damage done.
        playerCurrentHealth -= amount;
        Debug.Log("Player took " + amount);
        Debug.Log("Player's current health is " + playerCurrentHealth);
    }
}