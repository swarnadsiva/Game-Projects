using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateOpening : MonoBehaviour
{

    public bool GateIsOpen;// { get; set; }

    bool particlesActive = true;
    ParticleSystem[] myParticles;
    //Light lightComp;
    SphereCollider trigger;
    // Use this for initialization
    void Start()
    {
        GateIsOpen = false;
        
        trigger = GetComponent<SphereCollider>();
        trigger.enabled = false;

        myParticles = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem item in myParticles)
        {
            item.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GateIsOpen)
        {
            if (particlesActive)
            {
                foreach (ParticleSystem item in myParticles)
                {
                    item.Stop();
                }
                particlesActive = false;
            }
        }
        else
        {
            trigger.enabled = true;
            if (!particlesActive)
            {
                foreach (ParticleSystem item in myParticles)
                {
                    item.Play();
                }
                particlesActive = true;
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GateIsOpen)
        {
            if (other.gameObject.tag == GameObjectTag.Player.ToString())
            {
                if (GameController.Instance.GetCurrentGameScene() == ActiveGameScenes.Level1_New)
                {
                    GameController.Instance.checkpointLocation = Vector3.zero; //clear out the last checkpoint from level 1
                    GameController.Instance.StartCoroutine(GameController.Instance.GoToScene(ActiveGameScenes.Level3, GameState.Playing));

                }
                //else if (GameController.Instance.GetCurrentGameScene() == ActiveGameScenes.Level2)
                //{
                //    GameController.Instance.StartCoroutine(GameController.Instance.GoToScene(ActiveGameScenes.Level3, GameState.Playing));

                //}
                else if (GameController.Instance.GetCurrentGameScene() == ActiveGameScenes.Level3)
                {
                    GameController.Instance.StartCoroutine(GameController.Instance.GoToScene(ActiveGameScenes.Credits, GameState.MainMenu));
                }
                else
                {
                    GameController.Instance.StartCoroutine(GameController.Instance.GoToMainMenu());
                    print("You are not currently in an active game scene and cannot transition to another level.");
                }

            }
        }
    }

}
