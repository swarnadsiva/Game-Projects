using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialoguePoint : MonoBehaviour
{

    GameObject player;
    UIController uiControl;
    UIDialogueController dialogueControl;
    GuardianPuzzle guardPuzzle;
    CutSceneController cutScenCont;
    Level3ExitController level3Exit;

    public bool recordingCutScenes = false;
    public string speaker = "Kala";
    public bool cutSceneStarted;
    public bool cutSceneEnded;
    public bool doOnce;
    public bool showOnTriggerONLY;
    public bool cutSceneFade;
    public float dialogueDelay;

    public bool showOnConditionAfterEnergyVesselCollected;
    public bool showAfterHasEnergyVesselNearEnergy;
    public bool showWithoutEnergyVesselnearEnergy;
    public bool showAfterFallingPillarHits;
    public bool showAfterFirstEnemy;
    public bool showOnGuardianPuzzleComplete;
    public bool showOnApproachLevelExitWithOneOrb;
    public bool showAfterAllEnergyOrbsCollected;
    public bool showAfterPlayerHasRangedWeapon;
    public bool showOnDefendOrbChallengeComplete;
    public bool showOnEnterLevel3;
    public bool showAfterBossAppears;
    public bool showOnBossDeath;

    bool doneOnce;
   public  bool inArea = false;
    bool startDialogueTimer = false;
    GameObject boss;
    public float timer = 0f;

    [Tooltip("The dialogue to be displayed")]
    public string[] dialogueText;

    // Use this for initialization
    void Start()
    {
        guardPuzzle = FindObjectOfType<GuardianPuzzle>();
        cutScenCont = GetComponent<CutSceneController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (cutScenCont != null)
            {
                // b/c the player spawns dynamically in the scene, we need to dynamically add the cut scene variables to disable player movement and make sure only the idle animation is playing
                cutScenCont.myCutScene.GetComponent<QuickCutsceneController>().disableWhileInCutscene[1] = player.GetComponent<PlayerMovementController>();
                cutScenCont.myCutScene.GetComponent<QuickCutsceneController>().cutsceneAnimators[0] = player.GetComponentInChildren<Animator>();
                cutScenCont.myCutScene.GetComponent<QuickCutsceneController>().cutsceneAnimatorVariables[0] = "IsWalking";
                cutScenCont.myCutScene.GetComponent<QuickCutsceneController>().cutsceneAnimatorVariableTargets[0] = false;
            }
        }
        if (uiControl == null)
        {
            uiControl = GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>();
        }
        if (dialogueControl == null)
        {
            dialogueControl = FindObjectOfType<UIDialogueController>();
        }

        if (inArea && !recordingCutScenes)
        {
            if (ConditionsAreTrue())
            {
                CheckShowDialogue();
            }
        }

        if (startDialogueTimer)
        {
            timer += Time.deltaTime;
            if (timer >= dialogueDelay)
            {
                startDialogueTimer = false;
                uiControl.StartCoroutine(uiControl.ShowDialogueText(true, dialogueText, speaker));
            }
        }

        if (cutScenCont != null)
        {
            if (cutScenCont.myCutScene.playingCutscene && !cutSceneStarted)
            {
                cutSceneStarted = true;
            }

            if (!cutScenCont.myCutScene.playingCutscene && cutSceneStarted && !cutSceneEnded)
            {
                cutSceneEnded = true;
            }
        }

        if (boss == null)
        {
            boss = GameObject.FindGameObjectWithTag("BossEnemy");
        }
    }

    public bool ConditionsAreTrue()
    {
        PlayerStatController playerStats = player.GetComponent<PlayerStatController>();
        bool canShowDialogue = false;

        // begin checking for all conditions
        // NOTE: do not need to check player proximity b/c this is covered in the triggerenter and exit events
        if (showOnConditionAfterEnergyVesselCollected)
        {
            if (InventoryController.Instance.PlayerHasEnergyVessel)
            {
                canShowDialogue = true;
            }
        }
        else if (showAfterHasEnergyVesselNearEnergy)
        {
            if (InventoryController.Instance.PlayerHasEnergyVessel)
            {
                canShowDialogue = true;
            }
        }
        else if (showWithoutEnergyVesselnearEnergy)
        {
            if (!InventoryController.Instance.PlayerHasEnergyVessel)
            {
                canShowDialogue = true;
            }
        }
        else if (showAfterFallingPillarHits)
        {
            if (playerStats.currentHealth != playerStats.startingHealth)
            {
                canShowDialogue = true;
            }
        }
        else if (showAfterFirstEnemy)
        {
            foreach (BasicEnemyController item in FindObjectsOfType<BasicEnemyController>())
            {
                if (Vector3.Distance(transform.position, item.transform.position) <= 40f)
                {
                    canShowDialogue = true;
                    cutScenCont.myCutScene.smoothFollowTarget[0] = item.transform;
                    break;
                }
            }
        }
        else if (showOnGuardianPuzzleComplete)
        {
            if (guardPuzzle.PuzzleComplete && guardPuzzle.CanPlayGuardCutScene)
            {
                canShowDialogue = true;
            }
        }
        else if (showOnDefendOrbChallengeComplete)
        {
            NewGoldenOrbController orb = FindObjectOfType<NewGoldenOrbController>();
            if (orb != null)
            {
                if (orb.AmbushStarted && orb.CanCollect)
                {
                    canShowDialogue = true;

                }
            }
        }
        else if (showOnApproachLevelExitWithOneOrb)
        {
            if (InventoryController.Instance.PlayerHasFirstEnergy || InventoryController.Instance.PlayerHasSecondEnergy || InventoryController.Instance.PlayerHasThirdEnergy)
            {
                canShowDialogue = true;
            }
        }
        else if (showAfterAllEnergyOrbsCollected)
        {
            if (FindObjectOfType<GateOpening>().GateIsOpen)
            {
                canShowDialogue = true;
            }
        }
        else if (showAfterPlayerHasRangedWeapon)
        {
            if (InventoryController.Instance.PlayerHasRangedWeapon)
            {
                canShowDialogue = true;
            }
        }
        else if (showAfterBossAppears)
        {
            if (boss != null)
            {
                if (boss.activeInHierarchy)
                {
                    canShowDialogue = true;
                }
            }
        }
        else if (showOnBossDeath)
        {
            if (FindObjectOfType<Level3ExitController>() != null)
            {
                if (FindObjectOfType<Level3ExitController>().LevelComplete)
                {
                    canShowDialogue = true;
                }
            }
        }
        return canShowDialogue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inArea = true;
            if (showOnTriggerONLY && !recordingCutScenes)
            {
                CheckShowDialogue();
            }
        }
    }

    void CheckShowDialogue()
    {
        if (doOnce)
        {
            if (!doneOnce)
            {
                doneOnce = true;
                ShowDialogue();
            }
        }
        else
        {
            ShowDialogue();
        }
    }

    void ShowDialogue()
    {
        player.GetComponent<PlayerMovementController>().enabled = false;
        player.GetComponentInChildren<Animator>().SetBool("IsWalking", false);

        // if there is a cut scene controller attached, play the related cut scene
        if (cutScenCont != null)
        {
            foreach (var item in FindObjectsOfType<QuickCutsceneController>())
            {
                if (item != cutScenCont.myCutScene)
                {
                    item.EndCutscene();
                }
            }

            if (cutSceneFade)
            {
                StartCoroutine(cutScenCont.StartCutSceneWithFade());
            }
            else
            {
                cutScenCont.StartCutScene();
            }
        }
        startDialogueTimer = true;
        GameController.Instance.currentDialoguePoint = this;
    }

    IEnumerator DelayDialogue()
    {
        yield return new WaitForSeconds(dialogueDelay);
        uiControl.StartCoroutine(uiControl.ShowDialogueText(true, dialogueText, speaker));

    }

    private void OnTriggerExit(Collider other)
    {
        inArea = false;
    }
}
