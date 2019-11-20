using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawnPoint : MonoBehaviour
{
    //public
    [Tooltip("The melee enemy prefab.")]
    public GameObject enemyPrefab;
    [Tooltip("The ranged enemy prefab.")]
    public GameObject rangeEnemyPrefab;
    [Tooltip("How long to wait before respawning this enemy after death.")]
    private float spawnDelay = int.MaxValue;

    [Tooltip("Is this a ranged enemy spawn point?")]
    public bool SpawnRanged;
    [Tooltip("Is this a melee enemy spawn point?")]
    public bool SpawnMelee;

    [Tooltip("Check if this should spawn immediately when the game starts.")]
    public bool DoSpawn = false;
    
    //private
    BasicEnemyController enemyMeleeStats;
    RangeEnemyController enemyRangeStats;
    GameObject areaEnemyMelee;
    GameObject areaEnemyRange;
    float spawnTimerM;
    float spawnTimerR;

    private void OnEnable()
    {
        if (DoSpawn)
        {
            if (SpawnMelee)
            {

                if (areaEnemyMelee == null)
                {
                    areaEnemyMelee = Instantiate(enemyPrefab, transform.position, transform.rotation);
                }
                enemyMeleeStats = areaEnemyMelee.GetComponent<BasicEnemyController>();
            }

            if (SpawnRanged)
            {
                //if (!SceneManager.GetActiveScene().Equals("Level1") && !SceneManager.GetActiveScene().Equals("Level2"))
                //{
                    if (areaEnemyRange == null)
                    {
                        areaEnemyRange = Instantiate(rangeEnemyPrefab, transform.position, transform.rotation);
                    }
                    enemyRangeStats = areaEnemyRange.GetComponent<RangeEnemyController>();
                //}
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SpawnMelee)
        {
            if (areaEnemyMelee != null && !areaEnemyMelee.activeInHierarchy)
            {
                //start spawn timer
                spawnTimerM += Time.deltaTime;
                if (spawnTimerM >= spawnDelay)
                {
                    areaEnemyMelee.SetActive(true);
                    enemyMeleeStats.ResetStats();
                    spawnTimerM = 0f;
                }
            }
        }

        if (SpawnRanged)
        {

                if (areaEnemyRange != null && !areaEnemyRange.activeInHierarchy)
                {
                    //start spawn timer
                    spawnTimerR += Time.deltaTime;
                    if (spawnTimerR >= spawnDelay)
                    {
                        areaEnemyRange.SetActive(true);
                        enemyRangeStats.ResetStats();
                        spawnTimerR = 0f;
                    }
                }
        }
    }
}
