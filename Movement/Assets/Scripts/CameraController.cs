using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    GameObject following;
    public float speed = 0.2f;

    public float offset = 5f;

    // Use this for initialization
    void Start()
    {
        // get player object
        while (!PlayableCharacterUtilities.isSetup)
        {
            // wait to be setup
        }
        following = PlayableCharacterUtilities.GetCurrentPlayer();

    }

    // Update is called once per frame
    void Update()
    {
        if (following != null && following.CompareTag("Player"))
        {

            Vector3 offsetVector = new Vector3(0, offset/2, -offset);

            transform.position = following.transform.position + offsetVector;

            transform.LookAt(following.transform);
          
        } else
        {
            // try to find new player
            following = PlayableCharacterUtilities.GetCurrentPlayer();
        }
    }
}
