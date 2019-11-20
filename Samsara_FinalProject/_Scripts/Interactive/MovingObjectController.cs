using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the base class for moving objects in the game.
/// </summary>
public class MovingObjectController : MonoBehaviour
{
    // the direction to move in
    public bool moveInX;
    public bool moveInY;
    public bool moveInZ;

    // add or subract the move amount
    public bool moveNegative; 

    // implement a pause (for platforms) before moving
    public bool pauseBeforeMoving;

    // the amount to move
    public float moveAmount;

    // if this object can damage the player
    public bool isHazard;

    // movement starting position
    Vector3 startPos;

    // Use this for initialization
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // only move during pause
        if (GameController.Instance.CurrentGameState == GameState.Playing)
        {
            if (moveInX)
            {
                if (moveNegative)
                {
                    transform.position = new Vector3(startPos.x - Mathf.PingPong(Time.time, moveAmount), transform.position.y, transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(startPos.x + Mathf.PingPong(Time.time, moveAmount), transform.position.y, transform.position.z);
                }
            }
            else if (moveInY)
            {
                if (moveNegative)
                {
                    transform.position = new Vector3(transform.position.x, startPos.y - Mathf.PingPong(Time.time, moveAmount), transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, startPos.y + Mathf.PingPong(Time.time, moveAmount), transform.position.z);
                }
            }
            else if (moveInZ)
            {
                if (moveNegative)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, startPos.z - Mathf.PingPong(Time.time, moveAmount));
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, startPos.z + Mathf.PingPong(Time.time, moveAmount));
                }
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isHazard)
        {
            if (collision.gameObject.tag == GameObjectTag.Player.ToString())
            {
                PlayerStatController playerStat = collision.gameObject.GetComponent<PlayerStatController>();
                if (playerStat != null)
                {
                    if (playerStat.currentHealth > 0)
                    {
                        playerStat.TakeDamage(1);
                    }
                }
            }
        }

    }
}
