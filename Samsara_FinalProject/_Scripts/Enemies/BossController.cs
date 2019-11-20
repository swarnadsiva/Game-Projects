using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{

    public GameObject player;
    public int moveSpeed = 2;
    [RangeAttribute(0.5f, 3.0f)]
    public float timeBetweenAttacks = 3f; // we can adjust the time between attacks for difficulty

    int attackDamage = 1; // we can adjust the amount the enemies attack with

    float startingHealth = 1000f;  // ten hits to kill
    [RangeAttribute(1, 5)]
    public int stepLength = 2;

    public float wanderRadius;
    public float wanderTimer;

    public AudioSource clang;
    public AudioSource clang2;

    //public GameObject healthCanvas;
    //public Slider healthSlider;

    //private int maxDist = 1;
    private int attackDist = 10;
    private int minDist = 20;
    private int maxDist = 20;
    private PlayerStatController playerStats; //we need this to check the player's health before attacking
    private float attackTimer; // counting up to the next attack
    private float spawnTimer;

    private Vector3 currentLoc;

    private float m_currentHealth = 10;

    public float CurrentHealth
    {
        get
        {
            return m_currentHealth;
        }
        set
        {

            m_currentHealth = value;
            //if (healthSlider != null)
            //{
            //    healthSlider.value = m_currentHealth;
            //}
            if (m_currentHealth <= 0)
            {
                //print("enemy dead");
            }
        }
    }


    private bool playerInAttackRange;
    private Transform target;
    private NavMeshAgent agent;
    private float timer;
    Animator anim;
    public GameObject enemyToSpawn1;
    public GameObject effect;

    public GameObject chopBox;
    public GameObject sweepBox;

    public float TEST;

    private float minionSpawnTimer = 30f;

    private float dmgMultiplier = 1.0f;

    bool doneOnce = false;

    void OnEnable()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;

    }

    private void Awake()
    {

        //CurrentHealth = startingHealth;
        TEST = CurrentHealth;
        //// if we forgot to set a reference to the player gameobject, we can get it from the tag
        //if (player == null)
        //{
        //    player = GameObject.FindGameObjectWithTag("Player");
        //}
        //playerStats = player.GetComponent<PlayerStatController>();
        ////if (healthCanvas == null)
        ////{
        ////healthCanvas = GetComponentInChildren<Canvas>().gameObject;
        ////}

    }

    private void FixedUpdate()
    {
        dmgMultiplier += (float)(GameStatManager.Instance.AttackDamage) / 4;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerStats = player.GetComponent<PlayerStatController>();
        }

        if (GameController.Instance.CurrentGameState == GameState.Playing)
        {
            agent.isStopped = false;
            attackTimer += Time.deltaTime;
            timer += Time.deltaTime;
            spawnTimer += Time.deltaTime;


            if (Vector3.Distance(transform.position, player.transform.position) <= minDist)
            {
                //healthCanvas.SetActive(true);
                agent.updatePosition = true;

                StalkPlayer();


                //need to have attacktimer implemented
                if ((Vector3.Distance(transform.position, player.transform.position) <= attackDist))
                {
                    //AttackPlayer();
                    agent.isStopped = true;
                    agent.updatePosition = false;
                    gameObject.transform.LookAt(player.transform);
                    //print("attacking player yooooooooooooooooooooo");
                    AttackPlayer();
                }

                if (spawnTimer >= minionSpawnTimer)
                {
                    Instantiate(effect, (transform.position + new Vector3(0, 0, -5)), transform.rotation);
                    Instantiate(enemyToSpawn1, (transform.position + new Vector3(0, 0, -5)), transform.rotation);
                    spawnTimer = 0f;
                }
            }


            else if (Vector3.Distance(transform.position, player.transform.position) > minDist)
            {
                //healthCanvas.SetActive(false);
                Wander();
            }

            //MoveHealthSliderToEnemy();

            if (m_currentHealth <= 0 && !doneOnce)
            {
                doneOnce = true;
                playDeath();
                FindObjectOfType<Level3ExitController>().OpenGate();
                
                //gameObject.SetActive(false);
                //Destroy (gameObject);

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

    public void StalkPlayer()
    {
        if (!anim.GetBool("IsWalking"))
            anim.SetBool("IsWalking", true);
        if (anim.GetBool("IsIdle"))
            anim.SetBool("IsIdle", false);
        anim.SetBool("IsChopping", false);
        anim.SetBool("IsSwiping", false);
        transform.LookAt(player.transform);
        agent.SetDestination(player.transform.position);
    }

    public void AttackPlayer()
    {
        if (anim.GetBool("IsWalking"))
            anim.SetBool("IsWalking", false);
        //if (!anim.GetBool("IsIdle"))
        //    anim.SetBool("IsIdle", true);
        if (playerStats.currentHealth > 0 && attackTimer >= timeBetweenAttacks)
        {
            int attackRandomizer = Random.Range(1, 20);

            if (attackRandomizer == 1 || attackRandomizer == 2)
            {
                print("boss chopping");
                //anim.SetBool("IsWalking", false);
                //anim.SetBool("IsChopping", true);
                if (anim.GetBool("IsIdle"))
                    anim.SetBool("IsIdle", false);
                anim.SetTrigger("Chop");
                // set player damage
                chopBox.SetActive(true);
                StartCoroutine(Reset());
                //playerStats.TakeDamage(attackDamage);
                //reset attack timer
                attackTimer = 0f;

                //popback placeholder?
                //transform.rotation = Quaternion.LookRotation(transform.position - player.transform.position);
                //transform.position += Vector3.forward * stepLength * 2;
            }

            if (attackRandomizer == 4 || attackRandomizer == 5)
            {
                print("boss sweeping");
                //anim.SetBool("IsWalking", false);
                if (anim.GetBool("IsIdle"))
                    anim.SetBool("IsIdle", false);
                anim.SetTrigger("Sweep");
                //anim.SetBool("IsSwiping", true);
                // set player damage
                sweepBox.SetActive(true);
                StartCoroutine(ResetHeavy());
                //playerStats.TakeDamage(attackDamage * 3);
                //reset attack timer
                attackTimer = 0f;

                //popback placeholder?
                //transform.rotation = Quaternion.LookRotation(transform.position - player.transform.position);
                //transform.position += Vector3.forward * stepLength * 2;
            }
        }
        else
        {
            //if (!anim.GetBool("IsIdle"))
            //    anim.SetBool("IsIdle", true);
            //TESTING ONLY print("player health " + playerHealth.currentHealth + "attack timer " + attackTimer);
        }

    }

    public void Wander()
    {
        anim.SetBool("IsWalking", true);
        anim.SetBool("IsChopping", false);
        anim.SetBool("IsSwiping", false);

        Vector3 newPosition = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(newPosition);
        timer = 0;
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
            if (m_currentHealth > 0)
                StartCoroutine(Recoil());
            print("enemyHit");
            if (GameController.Instance.currentGameOptions.sfxOn)
                clang.Play();
            CurrentHealth -= 1 * dmgMultiplier;
            //GameController.Instance.PlayerExperience += 20f;
        }

        if (other.tag == "PlayerHeavyWeapon")
        {
            if (m_currentHealth > 0)
                StartCoroutine(Recoil());
            print("enemyHit");
            if (GameController.Instance.currentGameOptions.sfxOn)
                clang.Play();
            CurrentHealth -= 2 * dmgMultiplier; ;
            //GameController.Instance.PlayerExperience += 20f;
        }

        if (other.tag == "PlayerRangedWeapon")
        {
            if (m_currentHealth > 0)
                StartCoroutine(Recoil());
            print("enemyHit");
            if (GameController.Instance.currentGameOptions.sfxOn)
                clang2.Play();
            CurrentHealth -= 1;
            // GameController.Instance.PlayerExperience += 20f;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag.Equals(GameObjectTag.Player.ToString()))
        {
            //print("you got stabbed");

            // attack the player from the enemy script
            AttackPlayer();
        }
    }

    IEnumerator Disable()
    {
        anim.Play("BossDeath");
        agent.Stop();
        //anim.Play("Death");
        chopBox.SetActive(false);
        sweepBox.SetActive(false);
        yield return new WaitForSeconds(1.85f);
        FindObjectOfType<Level3ExitController>().LevelComplete = true;
        gameObject.SetActive(false);
    }

    IEnumerator Recoil()
    {
        anim.SetTrigger("Recoil");
        
        //anim.Play("BossRecoil");
        yield return new WaitForSeconds(1.5f);
        anim.ResetTrigger("Recoil");
        if (!anim.GetBool("IsIdle"))
            anim.SetBool("IsIdle", true);
    }

    private void playDeath()
    {
        StartCoroutine("Disable");
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(1.2f);
        chopBox.SetActive(false);
        //anim.SetBool("IsChopping", false);
        anim.ResetTrigger("Chop");
        anim.SetBool("IsIdle", true);
        //anim.SetBool("IsWalking", false);

    }

    IEnumerator ResetHeavy()
    {
        yield return new WaitForSeconds(1.2f);
        sweepBox.SetActive(false);
        //anim.SetBool("IsSwiping", false);
        anim.ResetTrigger("Sweep");
        anim.SetBool("IsIdle", true);
        //anim.SetBool("IsWalking", false);

    }

}
