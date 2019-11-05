using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPillar : MonoBehaviour
{

    public GameObject player;
    const float MIN_DISTANCE_TO_FALL = 5f;
    const float FALL_SPEED = 50f;

    public bool CanFall;
    public bool IsFalling { get; set; }
    public bool IsBeingDestroyed { get; set; }

    public AudioSource fallingPillar;

    public float randomFallSpeed;
    HingeJoint myJoint;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(LoadElements());
    }

    IEnumerator LoadElements()
    {
        yield return new WaitForSeconds(0.2f);
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        IsFalling = false;
        IsBeingDestroyed = false;
        randomFallSpeed = Random.Range(0.5f, 1) * FALL_SPEED;
        //if (Random.Range(0, 2) == 1)
        //{
        //    CanFall = true;
        //}
        //else
        //{
        //    CanFall = false;
        //    if (GetComponent<Rigidbody>() == null)
        //    {
        //        gameObject.AddComponent<Rigidbody>();
        //    }
        //    GetComponent<Rigidbody>().isKinematic = true;
        //    GetComponent<Rigidbody>().useGravity = false;
        //}

        // allow for users to set if the pillar can fall 
        if (!CanFall)
        {
            if (GetComponent<Rigidbody>() == null)
            {
                gameObject.AddComponent<Rigidbody>();
            }
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CanFall && player != null)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= MIN_DISTANCE_TO_FALL && !IsBeingDestroyed)
            {
                StartCoroutine(Fall());
            }

            if (IsBeingDestroyed)
            {

                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<Rigidbody>().useGravity = false;
                //transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), Time.deltaTime * 2f);
            }
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (CanFall)
        {
            if (IsFalling && !IsBeingDestroyed)
            {
                if (other.gameObject.tag == "Player")
                {
                    player.GetComponent<PlayerStatController>().TakeDamage(1);
                    IsBeingDestroyed = true;
                    //destroy immediately so the pillar isn't floating in mid-air
                    Destroy(gameObject);

                    //StartCoroutine(RemoveFromScene());
                }
                else
                {
                    IsBeingDestroyed = true;
                    StartCoroutine(RemoveFromScene());
                }
            }
        }


    }

    IEnumerator Fall()
    {
        myJoint = GetComponent<HingeJoint>();

        // Get the vector to the player without the Y vector
        // Vector3 direction = (new Vector3(transform.position.x, 0.0f, transform.position.z) - new Vector3(player.transform.position.x, 0.0f, player.transform.position.z));
        Vector3 direction = player.transform.position - transform.position;
        direction = new Vector3(direction.x, 0, direction.z);
        Vector3 perpendicularAxis = Vector3.Cross(direction, Vector3.up);

        if (!IsFalling)
        {
            myJoint.axis = perpendicularAxis.normalized;

            myJoint.anchor = new Vector3(direction.normalized.x, myJoint.anchor.y, direction.normalized.z);
        }
        IsFalling = true;
        GetComponent<Rigidbody>().AddForce(direction.normalized * randomFallSpeed); // increase fallspeed to add difficulty

        yield return null;
    }

    IEnumerator RemoveFromScene()
    {
        if (!fallingPillar.isPlaying && GameController.Instance.currentGameOptions.sfxOn)
        {
            fallingPillar.Play();
        }
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
