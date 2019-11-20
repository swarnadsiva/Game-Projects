using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObjectController : MonoBehaviour
{

    private Rigidbody rbody;

    private bool startTimer = false;
    private float timePassed = 0f;
    private bool TouchingGround { get; set; }
    private bool IsBeingDropped { get; set; }
    private Vector3 m_dropInFrontPos;
    private float dropDistance = 1.5f;
    //private Vector3 endPos;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        TouchingGround = false;
        IsBeingDropped = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == (int)Layer.Floor)
        {
            TouchingGround = true;
            IsBeingDropped = false;
        }
    }

    private void Update()
    {
        if (TouchingGround)
        {
            rbody.useGravity = false;
            rbody.isKinematic = true;
        }

        if (IsBeingDropped)
        {
            rbody.useGravity = true;
            rbody.isKinematic = false;
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + m_dropInFrontPos.x, transform.position.y, transform.position.z + m_dropInFrontPos.z), Time.deltaTime * 2f);
            //transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + m_dropInFrontPos.x * dropDistance, transform.position.y, transform.position.z + m_dropInFrontPos.z * dropDistance), Time.deltaTime * 2f);
        }
    }

    public void PlayerIsDroppingObject(Vector3 dropInFrontPos)
    {
        TouchingGround = false;
        IsBeingDropped = true;
        //startPos = transform.position;
        //endPos = dropInFrontPos;
        m_dropInFrontPos = dropInFrontPos;
        // move the object in front of the player to be dropped <-- i don't like this
        //transform.position = new Vector3(transform.position.x + dropInFrontPos.x * 2, transform.position.y, transform.position.z + dropInFrontPos.z * 2);
    }

}
