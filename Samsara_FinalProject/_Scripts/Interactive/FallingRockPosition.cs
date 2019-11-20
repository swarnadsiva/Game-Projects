using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRockPosition : MonoBehaviour {

    public GameObject fallingRockPrefab;
    public bool DoSpawn;

    GameObject myRock;
    bool doOnce = false;

	// Use this for initialization
	void Start ()
    {
        DoSpawn = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (DoSpawn && !doOnce)
        {
            doOnce = true;
            myRock = Instantiate(fallingRockPrefab, transform.position, transform.rotation);
        }
	}

   
}
