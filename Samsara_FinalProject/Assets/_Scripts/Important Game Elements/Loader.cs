using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{

    public GameObject gameControlPrefab; // prefab for GameController
    public GameObject screenTransPrefab; // prefab for screen transitions
    public GameObject soundManagerPrefab; // singleton!
    public GameObject inputControlPrefab;
    public GameObject objectiveControlPrefab; //singleton
    public GameObject inventoryControlprefab; //singleton
    public GameObject statManagerPrefab; //singleton
    public GameObject notificationControlPrefab;


    // can also include soundmanager here since that will be a singleton as well


    private void Awake()
    {
        if (GameController.Instance == null)
        {
            Instantiate(gameControlPrefab);
        }
        if (GameObject.FindGameObjectWithTag("ScreenTransCanvas") == null)
        {
            Instantiate(screenTransPrefab);
        }
        if (GameObject.FindGameObjectWithTag("SoundManager") == null)
        {
            Instantiate(soundManagerPrefab);
        }
        if (FindObjectOfType<InputController>() == null)
        {
            Instantiate(inputControlPrefab);
        }
        if (FindObjectOfType<ObjectiveController>() == null)
        {
            Instantiate(objectiveControlPrefab);
        }
        if (FindObjectOfType<InventoryController>() == null)
        {
            Instantiate(inventoryControlprefab);
        }
        if (FindObjectOfType<GameStatManager>() == null)
        {
            Instantiate(statManagerPrefab);
        }
        if (FindObjectOfType<NotificationController>() == null)
        {
            Instantiate(notificationControlPrefab);
        }
    }
}
