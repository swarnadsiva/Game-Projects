﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 0.5f;

    #region Private Variables
    int floorLayer;     // floor layer
    Rigidbody myRB;     // rigid body of gameobject
    Vector3 movement;   // reference to movement vector
    float deltaX;       // float to store input Horizontal Axis
    float deltaZ;       // float to store input Vertical Axis
    Animator anim;      // animator componentn
    #endregion

    private void Awake()
    {
        floorLayer = LayerMask.GetMask("Floor");
        myRB = gameObject.GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Fixed Update is called once per frame, consistent frame rate
    void FixedUpdate()
    {

        deltaZ = Input.GetAxis("Vertical") * speed;
        deltaX = Input.GetAxis("Horizontal") * speed;

        if (gameObject.CompareTag("Playable"))
        {
            // this object is playable but cannot move
            // TODO make it do some normal AI behavior
        }
        else if (gameObject.CompareTag("Player"))
        {
            // allow movement
            MoveFromInput();

            Turning();

            Animating();
        }
    }

    private void MoveRigidBody(float deltaX, float deltaZ)
    {
        movement.Set(deltaX, 0f, deltaZ);

        // normalize movement and make proportional to time
        movement = movement.normalized * speed * Time.deltaTime;

        // move object to new position
        myRB.MovePosition(transform.position + movement);
    }

    public void MoveFromInput()
    {
        deltaZ *= Time.deltaTime;
        deltaX *= Time.deltaTime;

        transform.Translate(deltaX, 0, deltaZ);
    }

    /// <summary>
    /// Rotates object according to mouse.
    /// 
    /// Based on Unity Survival Shooter tutorial.
    /// </summary>
    private void Turning()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        // check raycast from camera to floor
        RaycastHit floorHit;

        if (Physics.Raycast(camRay, out floorHit, 100f, floorLayer))
        {
            Vector3 playerToMouse = floorHit.point - transform.position;

            // along floor plane only
            playerToMouse.y = 0f;

            // set rotation
            Quaternion rotation = Quaternion.LookRotation(playerToMouse);
            myRB.MoveRotation(rotation);
        }
    }

    private void Animating()
    {
        bool walking = deltaZ != 0f || deltaX != 0f;

        anim.SetBool("IsWalking", walking);
    }
}
