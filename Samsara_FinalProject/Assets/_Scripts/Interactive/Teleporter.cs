using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Teleporter : MonoBehaviour
{
    public bool IsActive;
    public bool Locked;
    public bool TempDeactivate = false;
    public bool CanTeleport = false;
    public bool IsTeleporting = false;

    public Teleporter GoToLocation;

    ParticleSystem[] myParticles;
    GameObject player;
    UIController uiControl;

    // Use this for initialization
    void Start()
    {
        myParticles = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem item in myParticles)
        {
            item.Stop();
        }
        IsActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < 15f)
            {
                if (!IsActive)
                {
                    foreach (ParticleSystem item in myParticles)
                    {
                        item.Play();
                    }
                    IsActive = true;
                }
            }
            else
            {
                if (IsActive)
                {
                    foreach (ParticleSystem item in myParticles)
                    {
                        item.Stop();
                    }
                    IsActive = false;
                }
            }
        }
        else
        {
            player = GameObject.FindGameObjectWithTag(GameObjectTag.Player.ToString());
        }

        if (uiControl == null)
        {
            uiControl = FindObjectOfType<UIController>();
        }

        if (CanTeleport && InputController.CheckTeleport() && !IsTeleporting)
        {
            TempDeactivate = true;
            IsTeleporting = true;

            StartCoroutine(Teleport(GoToLocation));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!TempDeactivate)
            {
                StartCoroutine(uiControl.ShowHUDInteractionText(true, "Teleport", "collect"));
                CanTeleport = true;
            }
        }
    }

    IEnumerator Teleport(Teleporter newLoc)
    {
        newLoc.TempDeactivate = true;
        uiControl.ScreenFadeOut();
        yield return new WaitForSeconds(1.5f);
        player.transform.position = (newLoc.transform.position); 
        //player.GetComponent<NavMeshAgent>().Warp(newLoc.transform.position);
        uiControl.ScreenFadeIn();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (Locked)
                Locked = false;
            TempDeactivate = false;
            CanTeleport = false;
            IsTeleporting = false;
            StartCoroutine(uiControl.ShowHUDInteractionText(false));
        }
    }
}
