using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    GameObject following;
    [Range(0f, 1f)]
    public float speed = 0.2f;
    [Range(0f, 3f)]
    public float angle = 0.75f;
    [Range(0f, 50f)]
    public float offset = 5f;

    // Use this for initialization
    void Start()
    {
        // get player object
        following = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (following != null)
        {
            Debug.Log("following player");
            Vector3 offsetVector = new Vector3(0, offset / angle, -offset);

            transform.position = following.transform.position + offsetVector;

            transform.LookAt(following.transform);
          
        } else
        {
            // try to find new player
            following = GameObject.FindGameObjectWithTag("Player");

        }
    }
}
