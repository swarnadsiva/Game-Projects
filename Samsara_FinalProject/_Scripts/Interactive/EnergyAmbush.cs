using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyAmbush : MonoBehaviour
{

    public GameObject areaEnergy;
    public bool spawnOnAreaEnergyDeactivated = true;
    public bool spawnOnTrigger;
    GameObject player;
    PlayerStatController playerStats;
    bool doneOnce = false;
    bool inArea = false;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(LoadElements());
        if (areaEnergy == null)
        {
            foreach (GameObject item in GameObject.FindGameObjectsWithTag("Energy"))
            {
                if (Vector3.Distance(item.transform.position, transform.position) <= 15f)
                {
                    areaEnergy = item;
                    break;
                }
            }
        }
    }

    IEnumerator LoadElements()
    {
        yield return new WaitForSeconds(0.2f);
        if (player == null)
        {
            player = GameController.Instance.GetLevelGameObjectByTag(GameObjectTag.Player);
        }
        playerStats = player.GetComponent<PlayerStatController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!doneOnce)
        {
            if (spawnOnAreaEnergyDeactivated)
            {
                if (areaEnergy != null)
                {
                    if (!areaEnergy.activeInHierarchy)
                    {
                        //start ambush but only do once
                        StartCoroutine(AmbushPlayer());
                    }
                }
                else
                {
                    print("area energy null in a spawn point");
                }
               
            }
            else if (spawnOnTrigger)
            {
                if (inArea)
                {
                    StartCoroutine(AmbushPlayer());
                }
            }
        }
    }

    IEnumerator AmbushPlayer()
    {
        doneOnce = true;
        // spawn enemy ambush
        GetComponent<EnemyAmbushPoint>().ActivateEnemies(true);
        GetComponent<EnemyAmbushPoint>().ChallengeInProgress = true;
        yield return null;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inArea = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inArea = true;
        }
    }
}
