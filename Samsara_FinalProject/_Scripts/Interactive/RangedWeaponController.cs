using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeaponController : MonoBehaviour
{

    public bool MoveToPosition;
    public Transform hoverPosition;
    float yPosInitial;
    float amplitude = 0.5f;
    float speed = 0.5f;

    float timer = 0f;

    private void Start()
    {
        yPosInitial = hoverPosition.position.y; 
    }

    // Update is called once per frame
    void Update()
    {
        if (MoveToPosition)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, hoverPosition.position, Time.deltaTime * 2f);
            if (timer >= 2f)
            {
                MoveToPosition = false;
                timer = 0;
            }

        }
        else
        {
            timer += Time.deltaTime;
            float newY = yPosInitial + amplitude * Mathf.Sin(speed * timer);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);
        }
    }

    private void OnDisable()
    {
        MoveToPosition = false;
    }

}