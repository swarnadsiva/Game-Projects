using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    //public
    [Tooltip("How much higher the camera will be above the player.")]
    public float yOffset = 8f;


    //private
    Vector3 offset;
    GameObject player;
    float xOffset = 1f;
    float zOffset = -15f;
    float timer = 0f;
    bool isInitialized = false;
    bool doneLerping = false;

    void LateUpdate()
    {
        if (isInitialized && doneLerping)
        {
            // move camera to player position with correct offset
            transform.position = player.transform.position + offset;
        }
        else if (isInitialized && !doneLerping)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x + xOffset, player.transform.position.y + yOffset, player.transform.position.z + zOffset), Time.deltaTime * 6f);

            if (timer >= 1f)
            {
                doneLerping = true;
            }
        }
        else
        {
            InitializeCamera();
        }
    }

    void InitializeCamera()
    {
        if (player != null)
        {
            // rotate to look at player from specied angle
            transform.rotation = Quaternion.LookRotation(player.transform.position);
            transform.rotation = Quaternion.Euler(25f, 0f, 0f);

            //// move the camera to the offset position
            //transform.position = new Vector3(player.transform.position.x + xOffset, player.transform.position.y + yOffset, player.transform.position.z + zOffset);

            //// save the offset position
            //offset = transform.position - player.transform.position;

            // save the offset position
            offset = new Vector3(player.transform.position.x + xOffset, player.transform.position.y + yOffset, player.transform.position.z + zOffset) - player.transform.position;
            isInitialized = true;
        }
        else
        {
            //get the player from the tag
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    private void OnDisable()
    {
        isInitialized = false;
        doneLerping = false;
        timer = 0f;
    }
}