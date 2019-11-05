using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// the platform will flash the correct color when the golden rock is placed on it
/// </summary>
public class RockPlatformController : MonoBehaviour
{

    public Material correctMaterial;
    public Material changeBackTestMaterial;

    public float colorChangeSpeed;

    public bool HasInteractiveRock { get; set; }
    public bool CanChangeColor { get; set; }
    private Renderer rend;
    

    public WallController LockedWall { get; set; } // reference to the wall that this platform unlocks (set by the wall controller)

    private void Awake()
    {
        rend = GetComponent<Renderer>();
       // originalMaterial = rend.material;
        
        CanChangeColor = true;
    }

    // Use this for initialization
    void Start()
    {
        HasInteractiveRock = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanChangeColor)
        {
            if (HasInteractiveRock)
            {
                rend.material.Lerp(rend.material, correctMaterial, Time.deltaTime * colorChangeSpeed);
             
            }
            else
            {
                // go back to the original material 
                rend.material.Lerp(rend.material, changeBackTestMaterial, Time.deltaTime * colorChangeSpeed);
            }
        }
        else
        {
            // the color of the platform should stay locked onto the correct material when the puzzle is solved
            rend.material.Lerp(rend.material, correctMaterial, Time.deltaTime * colorChangeSpeed);
        }

        if (LockedWall.AllPlatformsActivated && CanChangeColor)
        {

            StartCoroutine(StopColorChange());
        }
    }

    IEnumerator StopColorChange()
    {
        yield return new WaitForSeconds(1f);
        CanChangeColor = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block")) // the block is the interactive rock for now
        {
            HasInteractiveRock = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Block"))
        {
            HasInteractiveRock = false;
        }
    }
}
