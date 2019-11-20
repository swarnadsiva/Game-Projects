using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthSpin : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            gameObject.SetActive(false);
        }
    }
}
