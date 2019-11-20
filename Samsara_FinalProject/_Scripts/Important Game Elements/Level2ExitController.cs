using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2ExitController : MonoBehaviour
{

    //private
    List<GameObject> goldenOrbs = new List<GameObject>();
    public GateOpening gateOpening;
    public GameObject player;
    public bool PuzzleComplete = false;
    PlayerStatController playerStats;


    // Use this for initialization
    void Start()
    {
        // get references to all golden orbs while they are active in the beginning of the scene
        foreach (GameObject rock in GameObject.FindGameObjectsWithTag("GoldenRock"))
        {
            goldenOrbs.Add(rock);
        }
        gateOpening = FindObjectOfType<GateOpening>();
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerStats = player.GetComponent<PlayerStatController>();
        }
    }

    public IEnumerator RemoveOrbFromPlayer(Transform trans, GameObject orb, GameObject orbStatue)
    {
        // sets the orb at the position of the statue
        // and makes it begin casting the light ray
        if (!orb.activeInHierarchy)
        orb.SetActive(true);
        orb.transform.position = trans.position;
        orb.transform.rotation = trans.rotation;
        orb.GetComponent<EnergyRandomMovement>().enabled = false;
        orb.GetComponent<EnergyOrbController>().CanPickUp = false;
        orb.GetComponent<EnergyOrbController>().CastRay = true;
        orb.GetComponent<EnergyOrbController>().InStatue = true;

        // Inventory
        if (orb.name.ToLower() == "energy1" && InventoryController.Instance.PlayerHasFirstEnergy)
        {
            InventoryController.Instance.RemoveEnergyOrbFromInventory(CollectibleItems.Energy1, orbStatue);
        }
        else if (orb.name.ToLower() == "energy2" && InventoryController.Instance.PlayerHasSecondEnergy)
        {
            InventoryController.Instance.RemoveEnergyOrbFromInventory(CollectibleItems.Energy2, orbStatue);
        }
        else if (orb.name.ToLower() == "energy3" && InventoryController.Instance.PlayerHasThirdEnergy)
        {
            InventoryController.Instance.RemoveEnergyOrbFromInventory(CollectibleItems.Energy3, orbStatue);
        }

        yield return null;
    }

    public IEnumerator OpenGate()
    {
        if (!PuzzleComplete)
            PuzzleComplete = true;
        gateOpening.GateIsOpen = true;
        InventoryController.Instance.Level1ExitComplete = true;
        yield return null;
    }
}
