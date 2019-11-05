using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnPoint : MonoBehaviour
{

    public GameObject playerPrefab;
    //public GameObject checkpoint1;
    //public GameObject checkpoint2;
    //public GameObject checkpoint3;


    GameObject player;
    Vector3 startPos;
    CheckPointController cpc;

    public bool cp1;
    public bool cp2;
    public bool cp3;

    public void SpawnPlayer(Vector3 spawnLocation)
    {
        if (GameObject.FindGameObjectWithTag(GameObjectTag.Player.ToString()) == null)
        {
            player = Instantiate(playerPrefab, spawnLocation, transform.rotation);
            player.transform.Rotate(0, 180, 0);

        }
    }
}
