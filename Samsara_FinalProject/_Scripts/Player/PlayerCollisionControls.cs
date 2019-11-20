using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class PlayerCollisionControls : MonoBehaviour
{

    public PlayerInteractionController interaction;
    public PlayerMovementController movement;
    private PlayerStatController playerStats;


    public float heightFactor = 3.2f;
    public float speed;

    public GameObject target;

    public float dmgRate = 1.2f;
    private float nextDmg = 0f;

    //private float m_distanceTraveled = 0f;


    Vector3 End_Pos;
    Vector3 Start_Pos;
    float fraction_of_the_way_there;

    private bool nearFrontLadder = false;
    private bool nearRightLadder = false;
    private bool nearLeftLadder = false;
    private bool nearDownLadder = false;
    private bool nearBackLadder = false;
    //private bool canInteract;
    private bool navAgentOn = true;

    private Vector3 velocity = Vector3.zero;

    public bool CanInteract { get; set; }

    private Rigidbody playerRigidBody;

    //private GameController gameControl;
    private UIController uiControl;


    private void Start()
    {
        Start_Pos = transform.position;
        End_Pos = transform.position + new Vector3(0, 0, 100);

        playerRigidBody = GetComponent<Rigidbody>();

        if (interaction == null)
        {
            interaction = GetComponentInParent<PlayerInteractionController>();
        }

        playerStats = GetComponentInParent<PlayerStatController>();
        CanInteract = true;
        StartCoroutine(LoadGameElements());
    }

    IEnumerator LoadGameElements()
    {
        yield return new WaitForSeconds(0.2f); // wait until the game level loader has been loaded
        uiControl = GameController.Instance.GetLevelGameObjectByTag(GameObjectTag.UIController).GetComponent<UIController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyProjectile" && Time.time > nextDmg)
        {
            //print("hit by arrow");
            playerStats.TakeDamage(1);
            Destroy(other.gameObject);

            nextDmg = Time.time + dmgRate;
        }

        if (other.tag == "EnemyMeleeWeapon" && Time.time > nextDmg)
        {
            //print("hit by melee");
            playerStats.TakeDamage(1);

            nextDmg = Time.time + dmgRate;
        }

        if (other.tag == "EnemyMeleeSweep" && Time.time > nextDmg)
        {
            //print("hit by melee");
            playerStats.TakeDamage(2);

            nextDmg = Time.time + dmgRate;
        }

        if (other.tag == "BossAxe" && Time.time > nextDmg)
        {
            //print("hit by melee");
            playerStats.TakeDamage(1);

            nextDmg = Time.time + dmgRate;
        }

        if (other.tag == "BossAxeSweep" && Time.time > nextDmg)
        {
            //print("hit by melee");
            playerStats.TakeDamage(2);

            nextDmg = Time.time + dmgRate;
        }

        if (other.tag == "Hazard" && Time.time > nextDmg * 2)
        {
            playerStats.TakeDamage(1);

            nextDmg = Time.time + dmgRate;
        }

        if (other.tag == "HealthPickup")
        {
            if (playerStats.currentHealth <= 4)
            {
                playerStats.Heal(3);
                other.gameObject.SetActive(false);
                //other.gameObject.GetComponent<ParticleSystem>().Stop();

            }

            if (playerStats.currentHealth == 5)
            {
                playerStats.Heal(2);
                other.gameObject.SetActive(false);
                //other.gameObject.GetComponent<ParticleSystem>().Stop();

            }

            if (playerStats.currentHealth == 6)
            {
                playerStats.Heal(1);
                other.gameObject.SetActive(false);
                //other.gameObject.GetComponent<ParticleSystem>().Stop();

            }

            print("full health!");

            //other.gameObject.SetActive(false);
            //other.gameObject.GetComponent<ParticleSystem>().Stop();

        }

        if (other.tag == "NavOff")
        {
            if (navAgentOn == true)
            {
                gameObject.GetComponent<NavMeshAgent>().enabled = false;
                navAgentOn = false;
            }

            else
            {
                gameObject.GetComponent<NavMeshAgent>().enabled = true;
                navAgentOn = true;
            }
        }

        if (other.tag == "NavOn")
        {
            if (navAgentOn == true)
            {
                gameObject.GetComponent<NavMeshAgent>().enabled = false;
                navAgentOn = false;
            }

            else
            {
                gameObject.GetComponent<NavMeshAgent>().enabled = true;
                navAgentOn = true;
            }
        }

        if (CanInteract)
        {
            if (other.tag == "ClimbableForward")
            {
                print("ayy");
                StartCoroutine(uiControl.ShowHUDInteractionText(true, "Climb", "climb"));
                nearFrontLadder = true;
            }

            if (other.tag == "ClimbableRight")
            {
                print("ayy");
                //movement.enabled = false;
                StartCoroutine(uiControl.ShowHUDInteractionText(true, "Climb", "climb"));
                nearRightLadder = true;
            }

            if (other.tag == "ClimbableLeft")
            {
                print("ayy");
                //movement.enabled = false;
                StartCoroutine(uiControl.ShowHUDInteractionText(true, "Climb", "climb"));
                nearLeftLadder = true;
            }

            if (other.tag == "ClimbableDown")
            {
                print("ayy");
                //movement.enabled = false;
                StartCoroutine(uiControl.ShowHUDInteractionText(true, "Climb", "climb"));
                nearDownLadder = true;
            }

            if (other.tag == "ClimbableBack")
            {
                print("ayy");
                //movement.enabled = false;
                StartCoroutine(uiControl.ShowHUDInteractionText(true, "Climb", "climb"));
                nearBackLadder = true;
            }

            if (other.gameObject.layer == (int)Layer.Movable) // movable layer
            {
                interaction.NearObject = true;
                //print(interaction.NearObject + " - near object");
                if (other.tag == "Energy")
                {
                    if (InventoryController.Instance.PlayerHasEnergyVessel && other.gameObject.GetComponent<EnergyOrbController>().CanPickUp)
                    {
                        StartCoroutine(uiControl.ShowHUDInteractionText(true, "Collect Energy", "collect"));
                    }
                    else
                    {
                        //uiControl.StartCoroutine(uiControl.ShowDialogueText(true, "You must collect the energy vessel."));
                        //uiControl.ShowHUDMessage(true, "You must find the energy vessel");
                    }
                }
                else if (other.tag == "GoldenRock" && other.GetComponent<NewGoldenOrbController>().CanCollect)
                {

                    StartCoroutine(uiControl.ShowHUDInteractionText(true, "Pick up Golden Orb", "collect"));


                }
                else if (other.tag == "EnergyVessel")
                {
                    StartCoroutine(uiControl.ShowHUDInteractionText(true, "Pick up Energy Vessel", "collect"));
                }
                else if (other.tag == "Block" && !interaction.HoldingObject)
                {
                    StartCoroutine(uiControl.ShowHUDInteractionText(true, "Pick up", "collect"));

                }
                else if (other.tag == "PlayerRangedWeapon")
                {
                    StartCoroutine(uiControl.ShowHUDInteractionText(true, "Collect Guardian's Gift", "collect"));
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (CanInteract)
        {

            if (other.tag == "ClimbableForward")
            {
                print("ayy");
                //movement.enabled = false;
                StartCoroutine(uiControl.ShowHUDInteractionText(false));
                nearFrontLadder = false;
            }

            if (other.tag == "ClimbableRight")
            {
                print("ayy");
                //movement.enabled = false;
                StartCoroutine(uiControl.ShowHUDInteractionText(false));
                nearRightLadder = false;
            }

            if (other.tag == "ClimbableLeft")
            {
                print("ayy");
                //movement.enabled = false;
                StartCoroutine(uiControl.ShowHUDInteractionText(false));
                nearLeftLadder = false;
            }

            if (other.tag == "ClimbableDown")
            {
                print("ayy");
                //movement.enabled = false;
                StartCoroutine(uiControl.ShowHUDInteractionText(false));
                nearDownLadder = false;
            }

            if (other.tag == "ClimbableBack")
            {
                print("ayy");
                //movement.enabled = false;
                StartCoroutine(uiControl.ShowHUDInteractionText(false));
                nearBackLadder = false;
            }


            if (other.gameObject.layer == (int)Layer.Movable && !interaction.HoldingObject)
            {
                //uiControl.ShowHUDInteractionText(false);
                uiControl.StartCoroutine(uiControl.ShowHUDInteractionText(false));
                interaction.NearObject = false;
            }
            //if (other.tag == "FirstToSecond")
            //{
            //    //uiControl.ShowHUDInteractionText(false);
            //}
        }
        else
        {
            if (GameController.Instance.HudInteractionShowing)
            {
                StartCoroutine(uiControl.ShowHUDInteractionText(false));
            }
        }
    }

    private void FixedUpdate()
    {
        float step = (speed * Time.deltaTime) / 2;

        Vector3 targetPosition1 = transform.position + (new Vector3(0, 2f, 0));
        Vector3 targetPosition2 = targetPosition1 + (new Vector3(0, 0, 2f));
        Vector3 origPosition = transform.position;


        //if ((nearFrontLadder == true && Input.GetKeyDown("e") || (nearFrontLadder == true && Input.GetKey("joystick button 0"))))
        if ((nearFrontLadder == true || (nearFrontLadder == true)) && InputController.CheckClimb())
        {
            //playerRigidBody.isKinematic = true;
            //print("ayy yo");
            //playerRigidBody.velocity = new Vector3(0, 5, 5);

            //transform.position = Vector3.Lerp(origPosition, targetPosition, .5f* Time.deltaTime);
            //playerRigidBody.MovePosition(targetPosition1);
            //playerRigidBody.MovePosition(targetPosition2);

            climbForwardUp();

        }

        if ((nearRightLadder == true || (nearRightLadder == true)) && InputController.CheckClimb())
        {
            //playerRigidBody.isKinematic = true;
            //print("ayy yo");
            //playerRigidBody.velocity = new Vector3(0, 5, 5);

            //transform.position = Vector3.Lerp(origPosition, targetPosition, .5f* Time.deltaTime);
            //playerRigidBody.MovePosition(targetPosition1);
            //playerRigidBody.MovePosition(targetPosition2);

            climbRightUp();

        }

        if ((nearLeftLadder == true || (nearLeftLadder == true)) && InputController.CheckClimb())
        {
            //playerRigidBody.isKinematic = true;
            //print("ayy yo");
            //playerRigidBody.velocity = new Vector3(0, 5, 5);

            //transform.position = Vector3.Lerp(origPosition, targetPosition, .5f* Time.deltaTime);
            //playerRigidBody.MovePosition(targetPosition1);
            //playerRigidBody.MovePosition(targetPosition2);

            climbLeftUp();
        }

        if ((nearBackLadder == true || (nearBackLadder == true)) && InputController.CheckClimb())
        {
            //playerRigidBody.isKinematic = true;
            //print("ayy yo");
            //playerRigidBody.velocity = new Vector3(0, 5, 5);

            //transform.position = Vector3.Lerp(origPosition, targetPosition, .5f* Time.deltaTime);
            //playerRigidBody.MovePosition(targetPosition1);
            //playerRigidBody.MovePosition(targetPosition2);

            climbBackUp();
        }

        else
        {
            playerRigidBody.isKinematic = false;
        }
    }

    public void DisableInteraction()
    {
        CanInteract = false;
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(5);
    }

    private void climbForwardUp()
    {
        iTween.MoveTo(gameObject, transform.position + new Vector3(0, 1.25f, 2), .8f);
    }

    private void climbBackUp()
    {
        iTween.MoveTo(gameObject, transform.position + new Vector3(0, 1.25f, -2), .8f);
    }

    private void climbRightUp()
    {
        iTween.MoveTo(gameObject, transform.position + new Vector3(2, 1.25f, 0), .8f);
    }

    private void climbLeftUp()
    {
        iTween.MoveTo(gameObject, transform.position + new Vector3(-2, 1.25f, 0), .8f);
    }
}
