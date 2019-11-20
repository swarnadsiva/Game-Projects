using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RangeEnemyController : MonoBehaviour {

    public GameObject player;
    public int moveSpeed = 2;
    [RangeAttribute(0.5f, 2.0f)]
    public float timeBetweenAttacks = 2f; // we can adjust the time between attacks for difficulty

    int attackDamage = 1; // we can adjust the amount the enemies attack with

    public float startingHealth = 2.0f;  // two hits to kill
    [RangeAttribute(1, 5)]
    public int stepLength = 2;

    public float wanderRadius;
    public float wanderTimer;

    public GameObject healthCanvas;
    public Slider healthSlider;

    public Rigidbody projectile;
    public int projSpeed = 10;

    public AudioSource swordClang;
    public AudioSource arrowClang;
    public AudioSource deathSound;

    //private int maxDist = 1;
    private int shootDist = 35;
    private PlayerStatController playerStats; //we need this to check the player's health before attacking
    private float attackTimer; // counting up to the next attack

    Animator anim;

    private float m_currentHealth;

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
                playDeath();
                //print("enemy dead");
                //gameObject.SetActive(false); //set inactive so we can just activate it later from spawn point
            }
        }
    }
    private bool playerInAttackRange;
    private Transform target;
    private NavMeshAgent agent;
    private float timer;

    private float dmgMultiplier = 1.0f;


    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;

    }

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
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
        dmgMultiplier += (float)(GameStatManager.Instance.AttackDamage) / 4;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }


        else
        {
            if (GameController.Instance.CurrentGameState == GameState.Playing)
            {
                agent.isStopped = false;
                attackTimer += Time.deltaTime;
                timer += Time.deltaTime;

                if (Vector3.Distance(transform.position, player.transform.position) <= shootDist)
                {
                    transform.LookAt(player.transform);
                    AttackPlayer();
                }
                else if (Vector3.Distance(transform.position, player.transform.position) > shootDist)
                {
                    Wander();
                }

                MoveHealthSliderToEnemy();
            }
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
    }


    public void StalkPlayer()
    {
        transform.LookAt(player.transform);
        //transform.position += transform.forward * moveSpeed * Time.deltaTime;
        agent.SetDestination(player.transform.position);
        //print("coming for player");
    }

    public void AttackPlayer()
    {

        // if player has health
        if ( attackTimer >= timeBetweenAttacks)
        {
            shoot();
            // set player damage
            //playerStats.TakeDamage(attackDamage);
            attackTimer = 0f;
        }
        else
        {
            //TESTING ONLY print("player health " + playerHealth.currentHealth + "attack timer " + attackTimer);
        }

    }

    public void Wander()
    {
        //print("starting wander");
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
            if (GameController.Instance.currentGameOptions.sfxOn)
            {
                swordClang.Play();

            }
            print("enemyHit");
            CurrentHealth -= 1 * dmgMultiplier;
            //GameController.Instance.PlayerExperience += 20f;
        }

        if (other.tag == "PlayerHeavyWeapon")
        {
            if (GameController.Instance.currentGameOptions.sfxOn)
            {
                swordClang.Play();

            }
            print("enemyHit");
            CurrentHealth -= 2 * dmgMultiplier; ;
            //GameController.Instance.PlayerExperience += 20f;
        }

        if (other.tag == "PlayerRangedWeapon")
        {
            if (GameController.Instance.currentGameOptions.sfxOn)
            {
                arrowClang.Play();
            }
            print("enemyHit");
            CurrentHealth--;
            //GameController.Instance.PlayerExperience += 20f;
        }
    }

    private void shoot()
    {
        anim.SetBool("IsWalking", false);
        anim.SetBool("IsShooting", true);

        Vector3 shootPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

        Rigidbody arrow = Instantiate(projectile, transform.position, transform.rotation);
        arrow.velocity = transform.TransformDirection(new Vector3(0, 0, projSpeed));

        StartCoroutine(Reset());

    }


    IEnumerator Disable()
    {
        anim.Play("RangeDeath");
        if (GameController.Instance.currentGameOptions.sfxOn)
        {
            deathSound.Play();
        }

        yield return new WaitForSeconds(.75f);
        gameObject.SetActive(false);
    }

    IEnumerator Recoil()
    {
        //anim.Play("BossRecoil");
        yield return new WaitForSeconds(1.2f);
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("IsShooting", false);
        anim.SetBool("IsWalking", false);
    }

    private void playDeath()
    {
        StartCoroutine("Disable");
    }

}
