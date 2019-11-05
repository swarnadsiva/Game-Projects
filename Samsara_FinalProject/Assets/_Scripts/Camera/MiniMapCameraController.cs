using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCameraController : MonoBehaviour
{
    //public
    public float miniMapHeight = 16f;

    //private
    GameObject player;
    Vector3 offset;
    bool isInitialized = false;

    void LateUpdate()
    {
        if (isInitialized)
        {
            // follow player
            transform.position = player.transform.position + offset;
        }
        else
        {
            InitializeMiniMapCamera();
        }

    }

    void InitializeMiniMapCamera()
    {
        if (player != null)
        {
            // set position and save as offset
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y + miniMapHeight, player.transform.position.z);
            offset = transform.position - player.transform.position;
            isInitialized = true;
        }
        else
        {
            //get the player from the tag
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }
}
