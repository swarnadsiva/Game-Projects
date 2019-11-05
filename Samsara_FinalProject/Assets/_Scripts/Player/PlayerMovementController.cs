using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovementController : MonoBehaviour
{

    #region Private Variables

    Vector3 m_Move;
    Animator anim;
    bool running = false;
    public bool controlsEnabled;
    private float nextDodge = 0f;

    #endregion

    #region Public Variables

    // made these public so that we can test it out in the editor
    [Range(1f, 20f)]
    public float speed = 1f;
    [Range(1f, 250f)]
    public float turnSpeed = .25f;
    public float DodgeRate = 1f;

    public AudioSource steps;

    #endregion

    private Rigidbody playerRigidBody;

    private PlayerStatController playerStats;

    private void Start()
    {
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(gameObject.transform.position, out closestHit, 500, 1))
        {
            //gameObject.transform.position = closestHit.position;
            //gameObject.AddComponent<NavMeshAgent>();
            //gameObject.GetComponent<NavMeshAgent>().radius = 1.5f;
            //gameObject.GetComponent<NavMeshAgent>().acceleration = -100;
            //gameObject.GetComponent<NavMeshAgent>().autoRepath = false;
            //gameObject.GetComponent<NavMeshAgent>().autoTraverseOffMeshLink = false;

        }

        playerRigidBody = GetComponent<Rigidbody>();
        controlsEnabled = true;
        playerStats = GetComponent<PlayerStatController>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!GameController.Instance.GodModeActive)
        {
            ////ray starts at player position and points down
            //Ray ray = new Ray(transform.position, Vector3.down);

            ////will store info of successful ray cast
            //RaycastHit hitInfo;

            ////terrain should have mesh collider and be on custom terrain 
            ////layer so we don't hit other objects with our raycast
            //LayerMask layer = 8 << LayerMask.NameToLayer("Floor");

            ////cast ray
            //if (Physics.Raycast(ray, out hitInfo, layer))
            //{
            //    //get where on the z axis our raycast hit the ground
            //    float z = hitInfo.point.z;

            //    //copy current position into temporary container
            //    Vector3 pos = transform.position;

            //    //change z to where on the z axis our raycast hit the ground
            //    pos.z = z;

            //    //override our position with the new adjusted position.
            //    transform.position = pos;
            //}

            if (controlsEnabled)
            {
                //if (Input.GetKeyDown(KeyCode.LeftShift))
                //{
                //    running = true;
                //    speed *= 2f;
                //}


                //if (Input.GetKeyUp(KeyCode.LeftShift))
                //{
                //    running = false;
                //    speed = 3.9f;
                //}
            }
        }
        else // in gode mode, allow player to fly up and down
        {
            if (Input.GetKey(KeyCode.RightBracket))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
            }

            if (Input.GetKey(KeyCode.Backslash))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
            }

            if (Input.GetKey(KeyCode.Return))
            {
                running = true;
                if (speed < 10)
                {
                    speed += 0.1f;
                }

            }


            if (Input.GetKey(KeyCode.RightShift))
            {
                running = false;
                if (speed > 0.1)
                {
                    speed -= 0.1f;
                }

            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                running = false;
                speed = 3.9f; // reset speed
            }
        }
    }

    private void FixedUpdate()
    {
        if (controlsEnabled)
        {
            // Get the velocity
            Vector3 horizontalMove = playerRigidBody.velocity;
            // Don't use the vertical velocity
            horizontalMove.y = 0;
            // Calculate the approximate distance that will be travelled
            float distance = horizontalMove.magnitude * Time.fixedDeltaTime;
            // Normalize horizontalMove since it should be used to indicate direction
            horizontalMove.Normalize();
            RaycastHit hit;

            // Check if the body's current velocity will result in a collision
            if (playerRigidBody.SweepTest(horizontalMove, out hit, distance))
            {
                // If so, stop the movement
                playerRigidBody.velocity = new Vector3(0, playerRigidBody.velocity.y, 0);
            }
            Movement();
        }

        if (!GameController.Instance.GodModeActive)
        {
            if (transform.position.y <= -50f && !GetComponent<PlayerStatController>().CheckDead()) // NO AREA IN OUR LEVELS CAN BE LOWER THAN THIS
            {
                // player is falling to their death
                GetComponent<PlayerStatController>().InstantDeath();
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    void Movement()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        if (moveHorizontal != 0 || moveVertical != 0)
        {
            if (!steps.isPlaying && GameController.Instance.currentGameOptions.sfxOn)
            {
                steps.Play();
            }

            anim.SetBool("IsWalking", true);
            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
            transform.Translate(movement * turnSpeed * Time.deltaTime * (speed + (GameStatManager.Instance.MovementSpeed / 10)), Space.World); //needs to use speed stat increase
        }
        else
        {
            steps.Stop();
            anim.SetBool("IsWalking", false);
        }

        //dodge for now
        if (InputController.CheckDodge() && Time.time > nextDodge)
        {
            playerRigidBody.AddForce(-transform.forward * 10, ForceMode.Impulse);
            //transform.Translate(m_Move.x,m_Move.y,m_Move.z-.5f);
            //transform.Translate(0,0,-.5f);
            nextDodge = Time.time + DodgeRate;
        }
    }

    public void SetInputControlsEnabled(bool isEnabled)
    {
        controlsEnabled = isEnabled;
    }
}

