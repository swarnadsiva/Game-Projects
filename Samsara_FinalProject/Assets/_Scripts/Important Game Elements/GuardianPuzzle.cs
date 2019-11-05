using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianPuzzle : MonoBehaviour
{
    //public 
    public TriggerTileController firstTileCont;
    public TriggerTileController secondTileCont;
    public TriggerTileController thirdTileCont;
    public DialoguePoint GuardianPuzzleCompleteDialoguePoint;

    public AudioSource grind;
    public AudioSource dropOrb;

    // public QuickCutsceneController headTurn;

    //public (not in editor)
    public bool OrderStarted { get; set; }
    public bool PuzzleComplete { get; set; }
    public bool CanPlayGuardCutScene { get; set; }

    //private
    GameObject guardian;
    GameObject player;
    Animator guardianAnim;
    RangedWeaponController rangeWeaponCont;
    CutSceneController cutSceneCont;
    bool firstCutSceneDone = false;
    bool secondCutSceneDone = false;
    bool thirdCutSceneDone = false;
    bool presentGuardGift = false;

    // Use this for initialization
    void Start()
    {
        OrderStarted = false;
        PuzzleComplete = false;
        CanPlayGuardCutScene = false;
        guardian = GameObject.FindGameObjectWithTag("Guardian");
        guardianAnim = guardian.GetComponent<Animator>();
        rangeWeaponCont = FindObjectOfType<RangedWeaponController>();
        if (rangeWeaponCont != null)
            rangeWeaponCont.gameObject.SetActive(false);
        cutSceneCont = GetComponent<CutSceneController>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((int)Time.time % 1 == 0) //check every 1 second
        {
            if (!PuzzleComplete)
            {
                if (!OrderStarted)
                {
                    CheckFirstTileSteppedInOrder();
                }
                else
                {
                    CheckSecondTileStepped();

                    CheckThirdTileStpped();

                    CheckPuzzleComplete();

                    if (PuzzleComplete)
                    {
                        return; //return before checking if the puzzle needs to be reset
                    }

                    CheckPuzzleNeedsToBeReset();

                }
            }
            else
            {
                if (!presentGuardGift && GuardianPuzzleCompleteDialoguePoint.cutSceneEnded)
                {
                    presentGuardGift = true;
                    StartCoroutine(Complete());
                }
            }
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (cutSceneCont != null)
            {
                // b/c the player spawns dynamically in the scene, we need to dynamically add the cut scene variables to disable player movement and make sure only the idle animation is playing
                cutSceneCont.myCutScene.GetComponent<QuickCutsceneController>().disableWhileInCutscene[1] = player.GetComponent<PlayerMovementController>();
                cutSceneCont.myCutScene.GetComponent<QuickCutsceneController>().cutsceneAnimators[0] = player.GetComponentInChildren<Animator>();
                cutSceneCont.myCutScene.GetComponent<QuickCutsceneController>().cutsceneAnimatorVariables[0] = "IsWalking";
                cutSceneCont.myCutScene.GetComponent<QuickCutsceneController>().cutsceneAnimatorVariableTargets[0] = false;
            }
        }

        if (cutSceneCont != null)
        {
            if (firstCutSceneDone && secondCutSceneDone && thirdCutSceneDone)
            {
                if (!CanPlayGuardCutScene)
                    StartCoroutine(WaitBeforeReEnabling());
            }
        }
    }

    /// <summary>
    /// Begins the order started if the player steps on the first tile.
    /// </summary>
    void CheckFirstTileSteppedInOrder()
    {
        if (firstTileCont.PlayerOnTile && !secondTileCont.PlayerOnTile && !thirdTileCont.PlayerOnTile)
        {
            OrderStarted = true;
            if (!firstCutSceneDone)
            {
                firstCutSceneDone = true;
                cutSceneCont.StartCutScene();
            }
            if (GameController.Instance.currentGameOptions.sfxOn)
            {
                if (!grind.isPlaying)
                    grind.Play();
            }

            else
            {
                grind.Stop();
            }
            guardianAnim.SetTrigger("FirstTileStepped");
            firstTileCont.StayChangedColor = true;
        }
    }

    /// <summary>
    /// Checks if the second tile has been stepped after the first tile, but before the third tile.
    /// </summary>
    void CheckSecondTileStepped()
    {
        // player must step on second and third in order
        if (firstTileCont.StayChangedColor && secondTileCont.PlayerOnTile && !thirdTileCont.PlayerOnTile)
        {
            if (!secondCutSceneDone)
            {
                secondCutSceneDone = true;
                cutSceneCont.StartCutScene();
            }
            if (GameController.Instance.currentGameOptions.sfxOn)
            {
                if (!grind.isPlaying)
                    grind.Play();
            }

            else
            {
                grind.Stop();
            }
            guardianAnim.SetTrigger("SecondTileStepped");
            secondTileCont.StayChangedColor = true;
        }
    }

    /// <summary>
    /// Checks if the third tile has been stepped in correct order.
    /// </summary>
    void CheckThirdTileStpped()
    {
        if (firstTileCont.StayChangedColor && secondTileCont.StayChangedColor && thirdTileCont.PlayerOnTile)
        {
            if (!thirdCutSceneDone)
            {
                thirdCutSceneDone = true;
                cutSceneCont.StartCutScene();
            }
            if (GameController.Instance.currentGameOptions.sfxOn)
            {
                if (!grind.isPlaying)
                    grind.Play();
            }

            else
            {
                grind.Stop();
            }
            guardianAnim.SetTrigger("ThirdTileStepped");
            thirdTileCont.StayChangedColor = true;
        }
    }

    /// <summary>
    /// Checks if all the tiles have been stepped on in the correct order.
    /// </summary>
    void CheckPuzzleComplete()
    {
        if (firstTileCont.StayChangedColor && secondTileCont.StayChangedColor && thirdTileCont.StayChangedColor)
        {
            //StartCoroutine(Complete());
            PuzzleComplete = true;
        }
    }

    /// <summary>
    /// Checks if the puzzle needs to be reset.
    /// </summary>
    void CheckPuzzleNeedsToBeReset()
    {
        if (firstTileCont.StayChangedColor && thirdTileCont.PlayerOnTile && !thirdTileCont.StayChangedColor && !PuzzleComplete)
        {
            guardianAnim.SetTrigger("Reset");
            OrderStarted = false;
            firstTileCont.StayChangedColor = false;
        }
    }

    /// <summary>
    /// Starts the process for the guardian to give the player the ranged weapon.
    /// </summary>
    /// <returns></returns>
    IEnumerator Complete()
    {
        //yield return new WaitForSeconds(10f); // this is where the dialogue/cut scene should happen
        if (GameController.Instance.currentGameOptions.sfxOn)
        {
            if (!dropOrb.isPlaying)
                dropOrb.Play();
        }

        else
        {
            dropOrb.Stop();
        }

        guardianAnim.SetTrigger("Complete");
        print("in complete coroutine");
        yield return new WaitForSeconds(1f);
        rangeWeaponCont.gameObject.SetActive(true);
        rangeWeaponCont.MoveToPosition = true;
        //PuzzleComplete = true;
    }

    IEnumerator WaitBeforeReEnabling()
    {
        yield return new WaitForSeconds(2f);
        CanPlayGuardCutScene = true;
    }

    public void EnsureGuardianPuzzleComplete()
    {
        firstTileCont.StayChangedColor = true;
        secondTileCont.StayChangedColor = true;
        thirdTileCont.StayChangedColor = true;
        GetComponentInChildren<Animator>().SetTrigger("Complete");
        PuzzleComplete = true;
        presentGuardGift = true;

    }
}

