using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Movement : MonoBehaviour {

    public float speed = 0.5f;

    int floorLayer;     // floor layer
    Rigidbody myRB;     // rigid body of gameobject
    Vector3 movement;   // reference to movement vector

	// Use this for initialization
	void Start () {
        floorLayer = LayerMask.GetMask("Floor");
        myRB = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

        float deltaZ = Input.GetAxis("Vertical") * speed;
        float deltaX = Input.GetAxis("Horizontal") * speed;

        Move(deltaX, deltaZ);
        //Rotate(deltaX);
        //MoveRigidBody(deltaX, deltaZ);

        RotateMouse();
	}

    private void MoveRigidBody(float deltaX, float deltaZ)
    {
        movement.Set(deltaX, 0f, deltaZ);

        // normalize movement and make proportional to time
        movement = movement.normalized * speed * Time.deltaTime;

        // move object to new position
        myRB.MovePosition(transform.position + movement);
    }

    private void Move(float deltaX, float deltaZ)
    {
        deltaZ *= Time.deltaTime;
        deltaX *= Time.deltaTime;

        transform.Translate(deltaX, 0, deltaZ);
    }

    private void Rotate(float deltaX)
    {
        float current = transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, deltaX, 0);
    }

    /// <summary>
    /// Rotates object according to mouse.
    /// 
    /// Based on Unity Survival Shooter tutorial.
    /// </summary>
    private void RotateMouse()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        // check raycast from camera to floor
        RaycastHit floorHit;
        if (Physics.Raycast(camRay, out floorHit, 100f, floorLayer)) {
            Vector3 playerToMouse = floorHit.point - transform.position;
            print("hit");
            // along floor plane only
            playerToMouse.y = 0f;

            // set rotation
            Quaternion rotation = Quaternion.LookRotation(playerToMouse);
            myRB.MoveRotation(rotation);
        }
    }
}
