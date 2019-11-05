using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour {

    Rigidbody rbody;

	// Use this for initialization
	void Awake () {
        rbody = GetComponent<Rigidbody>();
	}

    private void OnTriggerEnter(Collider other)
    {
        //means we have hit something
        if (other.gameObject.tag != "Player" && other.gameObject.layer != (int)Layer.Boundaries && other.gameObject.tag != "EnemyProjectile")
        {
            //print(other.gameObject.tag);
            rbody.useGravity = true;
        }
    }
}
