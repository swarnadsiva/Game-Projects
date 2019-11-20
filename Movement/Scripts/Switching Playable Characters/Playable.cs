using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playable : MonoBehaviour
{
    PlayerMovement movementScript;

    private void Awake()
    {
        movementScript = gameObject.GetComponent<PlayerMovement>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (movementScript == null)
        {
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
