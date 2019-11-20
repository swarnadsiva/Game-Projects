using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAmbushPoint : MonoBehaviour
{
    //public
    [Tooltip("Use if you want to specify one type of enemy.")]
    public GameObject enemyPrefab;
    [Tooltip("Use if you want to specify multiple types of enemies at specific locations in one ambush." +
        "NOTE: Cannot have both Enemy Prefab and Enemies To Activate specified.")]
    public List<GameObject> enemiesToActivate;
    [Tooltip("The number of enemies you want to spawn in the ambush.")]
    public int numberOfEnemies = 3;

    //public (not in editor)
    public bool ChallengeInProgress { get; set; }
    public bool ChallengeComplete { get; set; }

    //private
    float lowestFloorPos;
    GameObject player;
    PlayerStatController playerStats;

    private void Awake()
    {
        StartCoroutine(LoadElements());

        // deactivate enemies upon start
        ActivateEnemies(false);

        ChallengeComplete = false;
        ChallengeInProgress = false;
    }

    IEnumerator LoadElements()
    {
        yield return new WaitForSeconds(0.2f);
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        playerStats = player.GetComponent<PlayerStatController>();
        lowestFloorPos = int.MaxValue;
        foreach (GameObject floor in GameController.Instance.GetGameObjectsOnLayer(Layer.Floor))
        {
            if (floor.transform.position.y < lowestFloorPos)
            {
                lowestFloorPos = floor.transform.position.y;
            }
        }
    }

    private void Update()
    {
        if (ChallengeInProgress)
        {
            //check every 2 seconds if enemies are defeated
            if ((int)Time.time % 2 == 0)
            {
                ChallengeComplete = CheckAllEnemiesDefeated();
                if (ChallengeComplete)
                {
                    ChallengeInProgress = false; // stop checking
                }
            }
        }
    }

    /// <summary>
    /// Initializes and sets enemies to the specified active states.
    /// </summary>
    /// <param name="doActivate">Boolean indicating the enemies' active states. </param>
    public void ActivateEnemies(bool doActivate)
    {
        if (enemiesToActivate.Count > 0)
        {
            // user has specified specific enemies and positions, so use those
            for (int i = 0; i < enemiesToActivate.Count; i++)
            {
                if (enemiesToActivate[i] == null)
                {
                    enemiesToActivate[i] = Instantiate(enemyPrefab, GetRandomPosition(), transform.rotation);
                }
                enemiesToActivate[i].SetActive(doActivate);
            }
        }
        else
        {
            // user has specified a specific typ and numbere of enemy to spawn
            for (int i = 0; i < numberOfEnemies; i++)
            {
                enemiesToActivate.Add(Instantiate(enemyPrefab, GetRandomPosition(), transform.rotation));
                enemiesToActivate[i].SetActive(doActivate);
            }
        }

    }

    /// <summary>
    /// Retrieves a random position within a 10m radius for enemies to spawn.
    /// </summary>
    /// <returns>Random position as a Vector3.</returns>
    Vector3 GetRandomPosition()
    {
        Vector3 randPos = (Random.insideUnitSphere * 10f) + transform.position;
        while (randPos.y < lowestFloorPos)
        {
            randPos = (Random.insideUnitSphere * 10f) + transform.position;
        }
        return randPos;
    }

    /// <summary>
    /// Checks if all enemies in this ambush have been defeated (set inactive)
    /// </summary>
    /// <returns>Boolean indicating if all enemies have been defeated (set inactive)</returns>
     bool CheckAllEnemiesDefeated()
    {
        for (int i = 0; i < enemiesToActivate.Count; i++)
        {
            if (enemiesToActivate[i].activeInHierarchy)
            {
                // if any enemies are still active, they haven't been killed yet
                return false;
            }
        }
        // otherwise all the enemies have been killed
        return true;
    }

    public IEnumerator DeactivateMe()
    {
        foreach(GameObject enemy in enemiesToActivate)
        {
            enemy.SetActive(false);
        }
        gameObject.SetActive(false);
        yield return null;
    }
}
