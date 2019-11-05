using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this used to be attached to the player, but now i put it on each checkpoint area
public class CheckPointController : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            GameController.Instance.checkpointLocation = transform.position;

        }
    }
}
