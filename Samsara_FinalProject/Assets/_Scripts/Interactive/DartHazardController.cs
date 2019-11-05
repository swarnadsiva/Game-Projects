using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartHazardController : MonoBehaviour
{


    public List<GameObject> triggerTiles;
    public bool PlayerOnTriggerTile { get; set; }
    public bool PlayerInRange { get; set; }

    private List<TriggerTileController> triggerTileConts;
    private int frameCount = 0;

    private GameObject player;
    private PlayerStatController playerStats;

    private const float SHOOT_DELAY = 50f;
    private const float IN_RANGE_DISTANCE = 20f;
    private const int DAMAGE_AMOUNT = 5;
    private float timer = SHOOT_DELAY;

    private void Start()
    {
        // create list to store the tile controllers
        triggerTileConts = new List<TriggerTileController>();

        // iterate through the tile collection and store their controller components in our list
        foreach (GameObject triggerTile in triggerTiles)
        {
            triggerTileConts.Add(triggerTile.GetComponent<TriggerTileController>());
        }

        // initially, the player will not be on a trigger tile or within range of this hazard
        PlayerOnTriggerTile = false;
        PlayerInRange = false;

        // retrieve a reference to the player object and player stat controller
        //player = GameObject.FindGameObjectWithTag(GameObjectTag.Player.ToString());
        // playerStats = player.GetComponent<PlayerStatController>();
    }

    private void Update()
    {
        if (player != null)
        {
            // if the player is near this obstacle
            if (PlayerInRange)
            {
                // rotate the transform to look at the player
                transform.LookAt(player.transform);
            }

            // if the player is on any tile in the collection
            if (PlayerOnTriggerTile)
            {
                // begin incrementing the timer
                timer++;

                // if the timer is greater than the delay amount
                if (timer >= SHOOT_DELAY)
                {
                    // this means we can shoot the player
                    ShootPlayer();
                }
            }

            // only check every 4 frames to reduce load
            if (frameCount % 4 == 0)
            {
                PlayerInRange = CheckPlayerInRange();
            }


            // only check every 5 frames to reduce load
            if (frameCount % 5 == 0)
            {
                PlayerOnTriggerTile = CheckPlayerOnTriggerTile();
            }

            // increment the framecount
            frameCount++;
        }
        else
        {
            player = GameObject.FindGameObjectWithTag(GameObjectTag.Player.ToString());
            playerStats = player.GetComponent<PlayerStatController>();
        }
    }


    private bool CheckPlayerInRange()
    {
        //print(Vector3.Distance(transform.position, player.transform.position));
        // if the distance between this hazard and the player is less than the range distance

        if (Vector3.Distance(transform.position, player.transform.position) <= IN_RANGE_DISTANCE)
        {
            // the player is close, return true
            return true;
        }

        // the player is far away, return false
        return false;
    }

    bool CheckPlayerOnTriggerTile()
    {
        // check all the tile controllers this dart shooter responds to
        foreach (TriggerTileController tile in triggerTileConts)
        {
            // if the player is on any tile in this collection return true
            if (tile.PlayerOnTile)
            {
                return true;
            }
        }
        // if we have iterated through all the tiles
        // and they don't have the player on them,
        // return false
        return false;
    }

    private void ShootPlayer()
    {
        //reset timer
        timer = 0f;

        // shoot at player using raycast
        Vector3 fromPos = transform.position;

        // this line of code gets the local forward direction of our object
        Vector3 direction = transform.TransformVector(transform.worldToLocalMatrix.MultiplyVector(transform.forward));
        RaycastHit outhit;

        // debug only
        Debug.DrawRay(fromPos, direction, Color.yellow);
        if (Physics.Raycast(fromPos, direction, out outhit))
        {
            // the ray has hit the player
            if (outhit.collider.gameObject.CompareTag(GameObjectTag.Player.ToString()))
            {
                // if the player has more than 0 health
                if (playerStats.currentHealth > 0)
                {
                    // the player will take damage
                    playerStats.TakeDamage(DAMAGE_AMOUNT);
                }
            }
        }
    }
}
