using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatController : MonoBehaviour
{
    public int currentHealth;
    [Range(0, 10)]
    public int startingHealth = 7; // directly correlates to number of health spheres
    public Color hurtColor;

    public GameObject healthSpherePrefab;// healthspheres prefab
    public GameObject healthSpheres;

    PlayerMovementController playerMovement;
    PlayerCollisionControls playerCollision;
    UIController uiControl;
    Animator anim;

    public bool IsDamaged { get; set; }
    public bool CanPause { get; set; }

    public AudioSource hitSound;
    public AudioSource deathSound;

    // Use this for initialization
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        if (healthSpheres == null)
        {
            Instantiate(healthSpherePrefab);
            healthSpheres = GameObject.FindGameObjectWithTag(GameObjectTag.PlayerHealthSpheres.ToString());
            healthSpheres.transform.position = new Vector3(transform.position.x, transform.position.y + 3.5f, transform.position.z);
            healthSpheres.transform.SetParent(transform);
        }

        playerMovement = GetComponent<PlayerMovementController>();
        playerCollision = GetComponent<PlayerCollisionControls>();
        currentHealth = startingHealth;
        StartCoroutine(LoadGameElements());
    }

    IEnumerator LoadGameElements()
    {
        yield return new WaitForSeconds(0.2f); // wait until the game level loader has been loaded
        uiControl = GameController.Instance.GetLevelGameObjectByTag(GameObjectTag.UIController).GetComponent<UIController>();
    }

    public bool CheckDead()
    {
        if (currentHealth <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void TakeDamage(int amount)
    {
        if (!GameController.Instance.GodModeActive)
        {
            uiControl.ChangeHUDHurtImage();
            IsDamaged = true;

            anim.SetBool("IsHit", true);
            StartCoroutine(resetRecoil());

            for (int x = currentHealth - 1; x >= currentHealth - amount; x--)
            {
                healthSpheres.transform.GetChild(x).gameObject.SetActive(false);
            }
            currentHealth -= amount;

            if (GameController.Instance.currentGameOptions.sfxOn)
            {
                hitSound.Play();
            }

            if (CheckDead())
            {
                if (GameController.Instance.currentGameOptions.sfxOn)
                {
                    deathSound.Play();

                }
                anim.SetTrigger("Die");
                playerMovement.SetInputControlsEnabled(false);
                playerCollision.DisableInteraction();
                GameController.Instance.CurrentGameState = GameState.GameOver;
                uiControl.ShowGameOver();

            }

        }
    }

    public void Heal(int amount)
    {
        for (int x = currentHealth; x < currentHealth + amount; x++)
        {
            healthSpheres.transform.GetChild(x).gameObject.SetActive(true);
        }

        currentHealth += amount;
    }

    public void InstantDeath()
    {
        IsDamaged = true;
        currentHealth = 0;

        if (GameController.Instance.currentGameOptions.sfxOn)
            deathSound.Play();
        
        anim.Play("Death");
        anim.SetTrigger("Die");
        for (int i = 0; i < healthSpheres.transform.childCount; i++)
        {
            healthSpheres.transform.GetChild(i).gameObject.SetActive(false);
        }
        playerMovement.SetInputControlsEnabled(false);
        playerCollision.DisableInteraction();
        GameController.Instance.CurrentGameState = GameState.GameOver;
        uiControl.ShowGameOver();
    }

    IEnumerator resetRecoil()
    {
        yield return new WaitForSeconds(.2f);
        anim.SetBool("IsHit", false);
    }
}
