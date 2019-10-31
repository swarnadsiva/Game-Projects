using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Camera : MonoBehaviour
{

    public GameObject following;
    public float speed = 0.2f;

    public float offset = 5f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (following != null)
        {

            Vector3 offsetVector = new Vector3(0, offset/2, -offset);

            transform.position = following.transform.position + offsetVector;

            transform.LookAt(following.transform);
          
        }
    }
}
