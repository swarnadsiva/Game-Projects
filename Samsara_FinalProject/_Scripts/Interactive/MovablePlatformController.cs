using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatformController : MonoBehaviour
{
   // public GameObject boundaries;

    public GameObject moveUp; // +z
    public GameObject moveDown; // -z
    public GameObject moveRight; // +x
    public GameObject moveLeft; // -x

     float maxMoveAmountZ = 5;
     float maxMoveAmountX = 85;
    

    private GameObject player;

    public struct MoveControl
    {
        public GameObject controlObject;
        public TriggerTileController directionControl;
        public bool canMove;
        public float maxMoveAmount;
    }
    private MoveControl up, down, left, right;

    public bool PlayerOnPlatform { get; set; }
    public float PlatformSpeed { get; set; }

    // Use this for initialization
    void Start()
    {
        InitializeControls();

        player = GameObject.FindGameObjectWithTag(GameObjectTag.Player.ToString());
        PlayerOnPlatform = false;
        PlatformSpeed = 0.05f;
    }

    void InitializeControls()
    {
        up.controlObject = moveUp;
        up.directionControl = moveUp.GetComponent<TriggerTileController>();
        up.maxMoveAmount = transform.position.z + maxMoveAmountZ;
        up.canMove = true;

        down.controlObject = moveDown;
        down.directionControl = moveDown.GetComponent<TriggerTileController>();
        down.maxMoveAmount = transform.position.z - maxMoveAmountZ;
        down.canMove = true;

        right.controlObject = moveRight;
        right.directionControl = moveRight.GetComponent<TriggerTileController>();
        right.maxMoveAmount = transform.position.x + maxMoveAmountX;
        right.canMove = true;

        left.controlObject = moveLeft;
        left.directionControl = moveLeft.GetComponent<TriggerTileController>();
        left.maxMoveAmount = transform.position.x;
        left.canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (PlayerOnPlatform && GameController.Instance.CurrentGameState == GameState.Playing)
        {
            CheckBoundaries();

            if (up.directionControl.PlayerOnTile && up.canMove)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + PlatformSpeed);
            }

            if (down.directionControl.PlayerOnTile && down.canMove)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - PlatformSpeed);
            }

            if (left.directionControl.PlayerOnTile && left.canMove)
            {
                transform.position = new Vector3(transform.position.x - PlatformSpeed, transform.position.y, transform.position.z);
            }

            if (right.directionControl.PlayerOnTile && right.canMove)
            {
                transform.position = new Vector3(transform.position.x + PlatformSpeed, transform.position.y, transform.position.z);
            }
        }
    }

    private void CheckBoundaries()
    {
        right.canMove = transform.position.x < right.maxMoveAmount ? true : false;
        left.canMove = transform.position.x > left.maxMoveAmount ? true : false;
        up.canMove = transform.position.z < up.maxMoveAmount ? true : false;
        down.canMove = transform.position.z > down.maxMoveAmount ? true : false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameObjectTag.Player.ToString())
        {
            PlayerOnPlatform = true;
            player.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == GameObjectTag.Player.ToString())
        {
            PlayerOnPlatform = false;
            player.transform.SetParent(null);
        }
    }
}
