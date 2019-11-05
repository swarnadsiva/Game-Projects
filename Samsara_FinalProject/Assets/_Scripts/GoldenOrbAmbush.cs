using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenOrbAmbush : MonoBehaviour {

    GameObject areaOrb;
    GameObject player;
    PlayerStatController playerStats;
    bool doneOnce = false;

    // Use this for initialization
    void Start ()
    {
        StartCoroutine(LoadElements());
        if (areaOrb == null)
        {
            foreach (GameObject item in GameObject.FindGameObjectsWithTag(GameObjectTag.Energy.ToString()))
            {
                if (Vector3.Distance(item.transform.position, transform.position) <= 5f)
                {
                    areaOrb = item;
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
    void Update ()
    {
		if (!areaOrb.activeInHierarchy && !doneOnce)
        {
            //start ambush but only do once
           StartCoroutine(AmbushPlayer());
            print("ambush");
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
}
