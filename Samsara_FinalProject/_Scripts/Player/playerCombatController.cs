using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCombatController : MonoBehaviour
{
    public GameObject arrowSpawnPos;
    public GameObject swordPH;
    public GameObject swordArcPH;
    public GameObject playerProj;
    public GameObject crossbow;
    public int projSpeed;

    public float FireRate = 1f;
    private float nextFire = 0f;

    public float AttackRate = .50f;
    private float nextAttack = 0f;

    public GameObject[] targets;

    public AudioSource attack1;
    public AudioSource attack2;
    public AudioSource heavyAttackSnd;
    public AudioSource swoosh;
    public AudioSource cbow;

    private bool lockedOn = false;


    private GameObject myTarget;
    private int maxDist = 40;
    Animator anim;
    Vector3 lookTarget;

    // Use this for initialization
    void Awake()
    {
        crossbow.SetActive(false);
        swordPH.SetActive(false);
        swordArcPH.SetActive(false);
        anim = GetComponentInChildren<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.CurrentGameState == GameState.Playing)
        {
            if (InputController.CheckLightAttack() && Time.time > nextAttack)
            {
                int randomNum = UnityEngine.Random.Range(0, 2);

                if ( randomNum == 1 && GameController.Instance.currentGameOptions.sfxOn)
                {
                    attack1.Play();
                    swoosh.Play();

                }

                if (randomNum == 0 && GameController.Instance.currentGameOptions.sfxOn)
                {
                    attack2.Play();
                    swoosh.Play();
                }

                lightAttack();
                StartCoroutine("Wait");
                anim.SetBool("IsWalking", false);
                anim.SetBool("IsAttacking", true);

                StartCoroutine(Reset());
                nextAttack = Time.time + AttackRate;


                //anim.SetTrigger("Attack");
            }

            else if (InputController.CheckHeavyAttack() && Time.time > nextAttack)
            {
                if (GameController.Instance.currentGameOptions.sfxOn)
                {
                    heavyAttackSnd.Play();
                    swoosh.Play();
                }


                heavyAttack();
                //StartCoroutine("LongWait");
                anim.SetBool("IsWalking", false);
                anim.SetBool("IsAttackingHeavy", true);
                StartCoroutine(ResetHeavy());
                nextAttack = Time.time + AttackRate;

                //anim.SetTrigger("Attack");
            }

            else
            {
                swordPH.SetActive(false);
                swordArcPH.SetActive(false);
            }

            if (InputController.CheckRangedAttack() && Time.time > nextFire && InventoryController.Instance.PlayerHasRangedWeapon)
            {
                crossbow.SetActive(true);
                anim.SetBool("IsShooting", true);
                StartCoroutine(LongWait());
                StartCoroutine(playerShoot());
                StartCoroutine(ResetRange());
                nextFire = Time.time + FireRate;
            }

            if (InputController.CheckLockOnTarget())
            {
                myTarget = FindClosestEnemy();
                lookTarget = Vector3.zero;
                if (myTarget != null)
                {
                    lockedOn = true;
                    lookTarget = new Vector3(myTarget.transform.position.x, myTarget.transform.position.y, myTarget.transform.position.z);
                    //transform.LookAt(lookTarget);

                    var lookPos = myTarget.transform.position - transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = rotation;
                }

                else
                    lockedOn = false;
            }
        }
    }

    GameObject FindClosestEnemy()
    {
        GameObject temp = null;
        float shortestDistance = maxDist;
        targets = GameObject.FindGameObjectsWithTag("Enemy");
        if (targets.Length > 0)
        {
            foreach (GameObject enemy in targets)
            {
                float distance = Vector3.Distance(enemy.transform.position, transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    temp = enemy;
                }
            }
        }
        return temp;
    }

    private IEnumerator playerShoot()
    {
        //testing

        //Vector3 firePos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        //Rigidbody arrow = Instantiate(playerProj, firePos, transform.rotation);
        //arrow.velocity = transform.TransformDirection(new Vector3(0, 0, -projSpeed));
        //
        yield return new WaitForSeconds(0.2f);
        if (InventoryController.Instance.PlayerHasRangedWeapon && lockedOn == true)
        {

            Vector3 firePos = new Vector3(arrowSpawnPos.transform.position.x, arrowSpawnPos.transform.position.y - 1f, arrowSpawnPos.transform.position.z + 1.5f);
            GameObject arrow = Instantiate(playerProj, firePos, arrowSpawnPos.transform.rotation);
            arrow.transform.rotation = Quaternion.LookRotation(-arrow.transform.forward, Vector3.up);
            arrow.GetComponent<Rigidbody>().velocity = 30f * -arrow.transform.forward;
            if (GameController.Instance.currentGameOptions.sfxOn)
            {
                cbow.Play();
            }
            Destroy(arrow, 2.0f);
        }
    }

    //private void playerLockedShoot()
    //{
    //    if (InventoryController.Instance.PlayerHasRangedWeapon)
    //    {
    //        Vector3 firePos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
    //        GameObject arrow = Instantiate(playerProj, firePos, transform.rotation);
    //        arrow.GetComponent<Rigidbody>().velocity = 30f * arrow.transform.forward; //3f * transform.TransformDirection(new Vector3(0, 1, projSpeed));
    //        if (GameController.Instance.currentGameOptions.sfxOn)
    //        {
    //            cbow.Play();
    //        }

    //        Destroy(arrow, 2.0f);
    //    }
    //}

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(0.7f);
        anim.SetBool("IsAttacking", false);
        anim.SetBool("IsWalking", false);
    }

    IEnumerator ResetHeavy()
    {
        yield return new WaitForSeconds(0.7f);
        anim.SetBool("IsAttackingHeavy", false);
        //anim.SetBool("IsWalking", false);
    }

    IEnumerator ResetRange()
    {
        yield return new WaitForSeconds(0.7f);
        crossbow.SetActive(false);
        anim.SetBool("IsShooting", false);
    }


    private void lightAttack()
    {
        StartCoroutine("Wait");
        swordPH.SetActive(true);

    }


    private void heavyAttack()
    {
        StartCoroutine("LongWait");
        swordArcPH.SetActive(true);

    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.2f);
    }

    IEnumerator LongWait()
    {
        yield return new WaitForSeconds(1.5f);

    }
}

