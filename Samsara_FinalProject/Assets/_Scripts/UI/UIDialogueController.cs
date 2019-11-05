using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDialogueController : MonoBehaviour
{

    public bool isShowing = false;
    public bool isComplete = false;
    public UIController uiControl;
    public GameObject player;
    public Animator anim;

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance != null)
        {
            if (isShowing)
            {
                if (GameController.Instance.CurrentGameState == GameState.Playing)
                {
                    GameController.Instance.CurrentGameState = GameState.Paused;
                    GameController.Instance.CanPause = false;
                }

                // ensure all animations stop playing
                anim.SetBool("IsWalking", false);
                anim.SetBool("IsAttacking", false);
                anim.SetBool("IsPickup", false);
                anim.SetBool("IsShooting", false);

                if (InputController.CheckCloseDialogueMessage())
                {
                    uiControl.ShowNextDialogue();
                }
            }
            else if (!isShowing && isComplete)
            {
                if (GameController.Instance.CurrentGameState == GameState.Paused)
                {
                    GameController.Instance.CurrentGameState = GameState.Playing;
                    EnableMovementOnDialogueEnd();
                }
            }
        }

        if (uiControl == null)
        {
            uiControl = GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>();
        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            anim = player.GetComponentInChildren<Animator>();
        }
    }

    public IEnumerator DialogueComplete()
    {
        GameController.Instance.CanPause = true;
        isShowing = false;
        isComplete = true;
        yield return new WaitForSeconds(1f);
        isComplete = false; // needs to be reset so it will show other dialogue points
    }

    public void EnableMovementOnDialogueEnd()
    {
        if (player.GetComponent<PlayerMovementController>().enabled == false)
        {
            bool shouldEnable = true;
            foreach (QuickCutsceneController item in FindObjectsOfType<QuickCutsceneController>())
            {
                if (item.playingCutscene)
                {
                    shouldEnable = false;
                    break;
                }
            }
            if (shouldEnable)
            {
                player.GetComponent<PlayerMovementController>().enabled = true;
            }
        }
        else
        {
            player.GetComponent<PlayerMovementController>().SetInputControlsEnabled(true);
        }
           
    }
}
