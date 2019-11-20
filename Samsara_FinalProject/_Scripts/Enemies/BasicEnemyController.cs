using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BasicEnemyController : MonoBehaviour
{
    public GameObject player;
    public int moveSpeed = 2;
    [RangeAttribute(0.5f, 2.0f)]
    public float timeBetweenAttacks = 2f; // we can adjust the time between attacks for difficulty

    int attackDamage = 1; // we can adjust the amount the enemies attack with

    public float startingHealth = 100f;  // two hits to kill
    [RangeAttribute(1, 5)]
    public int stepLength = 2;

    public float wanderRadius;
    public float wanderTimer;

    public GameObject poundBox;
    public GameObject sweepBox;

    public GameObject healthCanvas;
    public Slider healthSlider;

    public AudioSource swordClang;
    public AudioSource arrowClang;
    public AudioSource deathSound;
    //public AudioSource enemyWalk;

    //private int maxDist = 1;
    public int minDist = 10;
    private int maxDist = 2;
    private float attackDist = 5f;
    private PlayerStatController playerStats; //we need this to check the player's health before attacking
    private float attackTimer; // counting up to the next attack

    private Vector3 currentLoc;

    public GameObject healthPickup;

    Animator anim;

    private float m_currentHealth;
    
    private CharacterController charControl;

    private float dmgMultiplier = 1.0f;

    public float CurrentHealth
    {
        get
        {
            return m_currentHealth;
        }
        set
        {

            m_currentHealth = value;
            if (healthSlider != null)
            {
                healthSlider.value = m_currentHealth;
            }
            if (m_currentHealth <= 0)
            {
                //print("enemy dead");
                //gameObject.SetActive(false); //set inactive so we can just activate it later from spawn point
                //anim.SetTrigger("Die");
                playDeath();
            }
        }
    }
    private bool playerInAttackRange;
    private Transform target;
    private NavMeshAgent agent;
    private float timer;

    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        StartCoroutine(LoadElements());
    }

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        charControl = GetComponent<CharacterController>();
        StartCoroutine(LoadElements());

    }

    IEnumerator LoadElements()
    {
        yield return new WaitForSeconds(0.2f);
        CurrentHealth = startingHealth;
        // if we forgot to set a reference to the player gameobject, we can get it from the tag
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        playerStats = player.GetComponent<PlayerStatController>();
        if (healthCanvas == null)
        {
            healthCanvas = GetComponentInChildren<Canvas>().gameObject;
        }
    }

    private void FixedUpdate()
    {

        dmgMultiplier += (float) (GameStatManager.Instance.AttackDamage)/4;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (GameController.Instance.CurrentGameState == GameState.Playing)
        {
            //print("game playing");
            agent.isStopped = false;
            attackTimer += Time.deltaTime;
            timer += Time.deltaTime;

            if (player != null)
            {
                //Wander();
                if (Vector3.Distance(transform.position, player.transform.position) <= minDist)
                {
                    healthCanvas.SetActive(true);
                    agent.isStopped = false; 
                    StalkPlayer();


                    if ((Vector3.Distance(transform.position, player.transform.position) <= attackDist))
                    {
                        AttackPlayer();
                        print("close to player");
                        moveSpeed = 0;
                        agent.isStopped=true;
                        AttackPlayer();
                    }
                }


                else if (Vector3.Distance(transform.position, player.transform.position) > minDist)
                {
                    //healthCanvas.SetActive(false);
                    agent.isStopped = false;
                    Wander();
                    //transform.position += transform.forward * moveSpeed * Time.deltaTime;
                }

                MoveHealthSliderToEnemy();
            }

        }
        else
        {
            agent.isStopped = true;

        }


    }

    public void ResetStats()
    {
        CurrentHealth = startingHealth;
    }

    public void MoveHealthSliderToEnemy()
    {
        if (healthSlider != null)
        {
            healthSlider.transform.position = Vector3.Lerp(healthSlider.transform.position, Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z)), Time.deltaTime * 25f);
        }
        else
        {
            print("health slider is null in an enemy.");
        }
    }


    public void StalkPlayer()
    {


        transform.LookAt(player.transform);
        //transform.position += transform.forward * moveSpeed * Time.deltaTime;
        anim.SetBool("isWalking", true);
        anim.SetBool("isAttackingPound", false);
        anim.SetBool("isAttackingBackhand", false);

        agent.SetDestination(player.transform.position);
        //print("coming for player");
        //if (!enemyWalk.isPlaying && GameController.Instance.currentGameOptions.sfxOn)
        //{
        //    enemyWalk.Play();

        //}
    }

    public void AttackPlayer()
    {
        //enemyWalk.Stop();
        anim.SetBool("isWalking", false);
        // if player has health
        if (playerStats.currentHealth > 0 && attackTimer >= timeBetweenAttacks)
        {
            int attackRandomizer = Random.Range(1, 12);


            if (attackRandomizer == 1 || attackRandomizer == 2)
            {
                //anim.Play("attackPound");
                //anim.SetBool("isWalking", false);
                if(!anim.GetBool("isAttackingPound") || !anim.GetBool("isAttackingBackhand"))
                    anim.SetBool("isAttackingPound", true);
                //StartCoroutine(doDamage(attackDamage));
                poundBox.SetActive(true);
                StartCoroutine(Reset());


                //reset attack timer
                attackTimer = 0f;

            }

            if (attackRandomizer == 4 || attackRandomizer == 5)
            {
                //anim.SetBool("isWalking", false);
                if (!anim.GetBool("isAttackingPound") || !anim.GetBool("isAttackingBackhand"))
                    anim.SetBool("isAttackingBackhand", true);
                //StartCoroutine(doDamage(attackDamage * 2));
                sweepBox.SetActive(true);
                StartCoroutine(ResetHeavy());

                //reset attack timer
                attackTimer = 0f;
            }
        }

        else
        {
            //anim.SetBool("isAttacking", true);
            anim.SetBool("isWalking", true);

            //TESTING ONLY print("player health " + playerHealth.currentHealth + "attack timer " + attackTimer);
        }

    }

    public void Wander()
    {


        if (timer >= wanderTimer)
        {
            //print("starting wander");
            anim.SetBool("isWalking", true);
            anim.SetBool("isAttackingBackhand", false);
            anim.SetBool("isAttackingPound", false);

            Vector3 newPosition = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPosition);
            timer = 0;
            //if (!enemyWalk.isPlaying && GameController.Instance.currentGameOptions.sfxOn)
            //{
            //    enemyWalk.Play();

            //}
        }

    }


    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * dist;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask);

        return navHit.position;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerMeleeWeapon")
        {
            if (GameController.Instance.currentGameOptions.sfxOn)
            {
                swordClang.Play();
            }

            //anim.Play("meleeRecoil");
            if (m_currentHealth > 0)
                anim.SetTrigger("Recoil");
            print("enemyHit");
            CurrentHealth-= 1* dmgMultiplier;
            //GameController.Instance.PlayerExperience += 20f;
            //SlamBack(new Vector3(player.transform.forward.x, player.transform.forward.y, player.transform.forward.z-2));
        }

        if (other.tag == "PlayerHeavyWeapon")
        {
            if (GameController.Instance.currentGameOptions.sfxOn)
            {
                swordClang.Play();
            }
            if (m_currentHealth > 0)
                anim.SetTrigger("Recoil");
            //anim.Play("meleeRecoil");
            print("enemyHit");
            CurrentHealth -= 2 * dmgMultiplier;
            //GameController.Instance.PlayerExperience += 20f;
            //SlamBack(new Vector3(player.transform.forward.x, player.transform.forward.y, player.transform.forward.z-2));
        }

        if (other.tag == "PlayerRangedWeapon")
        {
            if (GameController.Instance.currentGameOptions.sfxOn)
            {
                arrowClang.Play();
            }
            if(m_currentHealth > 0)
                anim.SetTrigger("Recoil");
            print("enemyHit");
            CurrentHealth -= 1;
            // GameController.Instance.PlayerExperience += 20f;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag.Equals(GameObjectTag.Player.ToString()))
        {
            // attack the player from the enemy script
            AttackPlayer();
        }
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(.8f);
        //yield return null;
    }

    IEnumerator waitLong()
    {
        yield return new WaitForSeconds(3f);
        //yield return null;
    }

    IEnumerator Disable()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isAttackingBackhand", false);
        anim.SetBool("isAttackingPound", false);

        anim.Play("death");
        agent.Stop();
        yield return new WaitForSeconds(.85f);

        poundBox.SetActive(false);
        sweepBox.SetActive(false);

        //anim.SetTrigger("Die");
        if (GameController.Instance.currentGameOptions.sfxOn)
        {
            deathSound.Play();
        }



        yield return new WaitForSeconds(1f);

        //anim.SetBool("isDead",true);

        int rand = Random.Range(1, 2);
        if (rand == 1 || rand ==2)
        {
            Instantiate(healthPickup, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(.85f);
        gameObject.SetActive(false);
        //anim.SetTrigger("Die");

        //yield return null;
    }

    private void playDeath()
    {
        StartCoroutine("Disable");
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(.8f);
        poundBox.SetActive(false);
        anim.SetBool("isAttackingPound", false);
        anim.SetBool("isWalking", false);

    }

    IEnumerator ResetHeavy()
    {
        yield return new WaitForSeconds(1f);
        sweepBox.SetActive(false);
        anim.SetBool("isAttackingBackhand", false);
        anim.SetBool("isWalking", false);

    }

    IEnumerator doDamage(int dam)
    {
        yield return new WaitForSeconds(4f);
        playerStats.TakeDamage(dam);
    }
}
