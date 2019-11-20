using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

    public GameObject[] managers;

    // Use this for initialization
    private void Awake()
    {
        LoadSingletonManagers();
    }

    private void LoadSingletonManagers()
    {
        foreach (GameObject manager in managers) {
            Debug.Log("loading manager");
            Instantiate(manager, transform.position, Quaternion.identity);
        }
    }
}
