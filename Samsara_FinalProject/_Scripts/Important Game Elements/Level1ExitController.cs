using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1ExitController : MonoBehaviour
{
    //// public 
    //[Tooltip("The positions that the energy orbs will float to when the player brings all three near.")]
    //public GameObject[] energyFinalPositions;

    //// private 
    //GameObject player;
    //GameObject[] energyOrbs;
    //GateOpening gateOpening;
    //PlayerStatController playerStats;
    //bool isLevelComplete = false;

    //private void Awake()
    //{
    //    // get references to all energy orbs while they are active in the beginning of the scene
    //    energyOrbs = GameObject.FindGameObjectsWithTag("Energy");
    //    for (int i = 0; i < energyOrbs.Length; i++)
    //    {
    //        // set each of their level end positions to the ones that have been specified 
    //        energyOrbs[i].GetComponent<EnergyRandomMovement>().EnergyLevelEndPosition = new Vector3(energyFinalPositions[i].transform.position.x, energyFinalPositions[i].transform.position.y + 2f, energyFinalPositions[i].transform.position.z);
    //    }
    //    gateOpening = FindObjectOfType<GateOpening>();

    //}

    //private void Update()
    //{
    //    if (player == null)
    //    {
    //        player = GameObject.FindGameObjectWithTag("Player");
    //        playerStats = player.GetComponent<PlayerStatController>();
    //    }

    //    if (GetComponent<EnemyAmbushPoint>().ChallengeComplete && !isLevelComplete)
    //    {
    //        StartCoroutine(Level1Complete());
    //    }
    //}


    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!GetComponent<EnemyAmbushPoint>().ChallengeComplete && !GetComponent<EnemyAmbushPoint>().ChallengeInProgress)
    //    {
    //        if (other.tag == "Player")
    //        {

    //            // if the player has the energy vessels and all three energies activate the enemies
    //            if (playerStats.HasEnergyVessel && playerStats.EnergiesCollected >= 3)
    //            {
    //                // spawn enemy ambush
    //                GetComponent<EnemyAmbushPoint>().ActivateEnemies(true);
    //                GetComponent<EnemyAmbushPoint>().ChallengeInProgress = true;
    //            }

    //        }
    //    }
    //}

    //IEnumerator LetEnergyMove(GameObject energyOrb)
    //{

    //    // set position to player
    //    energyOrb.transform.position = new Vector3(player.transform.position.x + 2, player.transform.position.y, player.transform.position.z);

    //    //deactivate random movement script
    //    energyOrb.GetComponent<EnergyRandomMovement>().MoveToEndPosition = true;

    //    // set active
    //    energyOrb.SetActive(true);
    //    yield return new WaitForSeconds(2f);
    //}

    //IEnumerator Level1Complete()
    //{
    //    // play camera event here?

    //    isLevelComplete = true;

    //    for (int i = 0; i < energyOrbs.Length; i++)
    //    {
    //        StartCoroutine(LetEnergyMove(energyOrbs[i]));
    //        playerStats.EnergiesCollected--;
    //        yield return new WaitForSeconds(1.5f);
    //    }

    //    //open the gate after all energy moves
    //    gateOpening.GateIsOpen = true;
    //}
}
