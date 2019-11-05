using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


#region Enumerations

public enum Layer { Floor = 8, Movable, Minimap, Boundaries };
public enum GameState { Paused, Playing, GameOver, MainMenu };
public enum ActiveGameScenes { CannotFind = -1, MainMenu, IntroScene, Level1, Level2, Level3, Level1_New, Credits };
public enum GameObjectTag { Player, UIController, MainCamera, EnemyMeleeWeapon, PlayerMeleeWeapon, Block, Energy, Enemy, EnergyVessel, Wall, GoldenRock, MovablePlatform, PlayerHealthSpheres, PlayerSpawnPoint, GateOpening, LightCatcher, Guardian, Teleporter };
public enum UITag { HUD, GameOverCanvas, PlayerHealthCanvas, PauseCanvas, MenuMainCanvas, MenuHelpCanvas, GodModeCanvas }
public enum InputType { Controller, Keyboard }
public enum GameOverAnimationTriggers { Show, Hide };

#endregion //Enumerations

/// <summary>
/// 
/// The GameController handles all functionality within the game.
/// This includes GameState management and appropriate delegation to handle
/// scene transitions (from level to level, menu to menu)
/// as well as UI events/transitions
/// 
/// 
/// This is also the location of constants, global variables, etc. that can be accessed
/// at any time from the other GameObject instances.
/// 
/// </summary>
public class GameController : MonoBehaviour
{
    #region Constants

    public const float DELAY_BETWEEN_SCENES = 2f;

    #endregion //Constants

    #region Editor Public Variables

    public bool GodModeActive;
    public Vector3 checkpointLocation;
    public bool defaultSFXSetting;
    public bool defaultMusicSetting;
    public bool defaultKeyboard;

    #endregion

    #region Global Public Variables 

    public DialoguePoint currentDialoguePoint;
    public struct GameOptions
    {
        public InputType inputSetting;
        public bool musicOn;
        public bool sfxOn;
    }
    public GameOptions currentGameOptions;

    private GameState m_currentGameState;
    public GameState CurrentGameState
    {
        get
        {
            return m_currentGameState;
        }
        set
        {
            m_currentGameState = value;
            switch (m_currentGameState)
            {
                case GameState.Paused:
                    {
                        PauseGame();
                        break;
                    }

                case GameState.Playing:
                    {
                        PlayGame();
                        break;
                    }

                case GameState.GameOver:
                    {

                        EndGame();
                        break;
                    }

                case GameState.MainMenu:
                    InMenu();
                    break;
                default:
                    break;
            }
        }
    }

    // static instance that can be accessed from any other gameobject
    public static GameController Instance { get; set; }

    // public InventoryItemDescriptions inventoryDesc;
    public bool HudInteractionShowing;
    public bool CutScenePlaying;
    public bool CanPause;

    public GameObject skipNotice;
    #endregion

    #region Private Member Variables

    GameObject player;
    GameObject playerSpawnPoint;
    PlayerStatController playerStats;
    UIController uiControl;
    UIDialogueController uiDialogueCont;

    bool canPress;
    public bool canSkip;
    public float skipTimer;
    #endregion

    private void Start()
    {

        // ensures there is only one instance of our GameObject at any given time.
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);



        // set default game options
        if (defaultKeyboard)
        {
            currentGameOptions.inputSetting = InputType.Keyboard;

        }
        else
        {
            currentGameOptions.inputSetting = InputType.Controller;
        }
        currentGameOptions.sfxOn = defaultSFXSetting;
        currentGameOptions.musicOn = defaultMusicSetting;

        LoadTheLevel();
    }

    private void Update()
    {
        if (CurrentGameState == GameState.Playing && InputController.CheckGodMode())
        {
            // allows toggling of god mode on and off
            if (GodModeActive)
            {
                GodModeActive = false;
                uiControl.ShowGodModeCanvas(false);
                if (player != null)
                {
                    player.GetComponent<Rigidbody>().useGravity = true;
                }
            }
            else
            {
                GodModeActive = true;
                uiControl.ShowGodModeCanvas(true);
                if (player != null)
                {
                    player.GetComponent<Rigidbody>().useGravity = false;
                }
            }
        }

        CutScenePlaying = CheckCutScenePlaying();

        if (uiControl != null)
        {
            if (CanPause && InputController.CheckPause() && !CutScenePlaying)
            {

                if (CurrentGameState == GameState.Paused)
                {
                    uiControl.ShowPauseOverlay(false);
                    CurrentGameState = GameState.Playing;
                }
                else if (CurrentGameState == GameState.Playing)
                {
                    uiControl.ShowPauseOverlay(true);
                    CurrentGameState = GameState.Paused;
                }
            }
        }
        else
        {
            uiControl = FindObjectOfType<UIController>();
        }

        if (uiDialogueCont != null)
        {
            if (CutScenePlaying && !uiDialogueCont.isShowing)
            {
                skipTimer += Time.deltaTime;
                if (InputController.CheckSkipCutscene())
                {
                    if (canSkip)
                    {
                        StopCutscene();
                        if (currentDialoguePoint != null)
                        {
                            currentDialoguePoint.timer = 0f;
                            currentDialoguePoint.dialogueDelay = 0.5f;
                        }
                    }
                    else
                    {
                        if (!skipNotice.activeInHierarchy)
                            skipNotice.SetActive(true);
                        if (skipTimer >= 0.5f)
                        {
                            canSkip = true;
                        }
                    }
                }
            }
            else if (!CutScenePlaying)
            {
                //reset here
                skipTimer = 0f;
                canSkip = false;
                skipNotice.SetActive(false);
            }
        }
        else
        {
            uiDialogueCont = FindObjectOfType<UIDialogueController>();
        }


        if (canPress)
        {

            if ((Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) && GetCurrentGameScene() != ActiveGameScenes.Level1_New)
            {
                //print("1 pressed");
                canPress = false;
                StartCoroutine(GoToScene(ActiveGameScenes.Level1_New, GameState.Playing));

            }

            if ((Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) && GetCurrentGameScene() != ActiveGameScenes.Level3)
            {
                //print("3 pressed");
                canPress = false;
                StartCoroutine(GoToScene(ActiveGameScenes.Level3, GameState.Playing));
            }
        }
    }

    void StopCutscene()
    {
        foreach (QuickCutsceneController item in FindObjectsOfType<QuickCutsceneController>())
        {
            if (item.playingCutscene)
            {
                item.EndCutscene();
            }
        }
    }

    bool CheckCutScenePlaying()
    {
        foreach (QuickCutsceneController item in FindObjectsOfType<QuickCutsceneController>())
        {
            if (item.playingCutscene)
            {
                return true;
            }
        }
        return false;
    }

    public void LoadTheLevel()
    {
        canPress = true;

        uiControl = FindObjectOfType<UIController>();

        if (GetCurrentGameScene() == ActiveGameScenes.MainMenu || GetCurrentGameScene() == ActiveGameScenes.IntroScene || GetCurrentGameScene() == ActiveGameScenes.Credits)
        {
            CurrentGameState = GameState.MainMenu;
        }
        else if (GetCurrentGameScene() == ActiveGameScenes.Level2 ||
            GetCurrentGameScene() == ActiveGameScenes.Level1_New ||
          GetCurrentGameScene() == ActiveGameScenes.Level1 ||
          GetCurrentGameScene() == ActiveGameScenes.Level3)
        {
            playerSpawnPoint = GetLevelGameObjectByTag(GameObjectTag.PlayerSpawnPoint);

            if (checkpointLocation != Vector3.zero && (GetCurrentGameScene() == ActiveGameScenes.Level1_New || GetCurrentGameScene() == ActiveGameScenes.Level3))
            {
                playerSpawnPoint.GetComponent<PlayerSpawnPoint>().SpawnPlayer(checkpointLocation);
            }
            else
            {
                playerSpawnPoint.GetComponent<PlayerSpawnPoint>().SpawnPlayer(playerSpawnPoint.transform.position);
            }

            // get all necessary components here
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerStats = player.GetComponent<PlayerStatController>();
            }

            InventoryController.Instance.VerifyInventoryItemsInLevel(); // deactivates objects/dialogue/puzzles the player has already found/completed

            CurrentGameState = GameState.Playing;
        }
        else // for test levels
        {
            // get all necessary components here
            player = GetLevelGameObjectByTag(GameObjectTag.Player);
            if (player != null)
            {
                playerStats = player.GetComponent<PlayerStatController>();
            }
            CurrentGameState = GameState.Playing;
        }
    }

    private void OnLevelWasLoaded()
    {
        // ensures that level is loaded
        LoadTheLevel();
    }

    #region Public Static Functions

    public void PauseGame()
    {
        player.GetComponent<PlayerCollisionControls>().DisableInteraction();
        player.GetComponent<PlayerMovementController>().SetInputControlsEnabled(false);

    }

    public void PlayGame()
    {
        player.GetComponent<PlayerCollisionControls>().CanInteract = true;
        player.GetComponent<PlayerMovementController>().SetInputControlsEnabled(true);
    }

    public void EndGame()
    {

    }

    public void InMenu()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public List<GameObject> GetGameObjectsOnLayer(Layer layer)
    {
        List<GameObject> temp = new List<GameObject>();
        foreach (GameObject item in GameObject.FindObjectsOfType<GameObject>())
        {
            if (item.layer == (int)Layer.Floor)
            {
                temp.Add(item);
            }
        }
        return temp;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Retrieves any UI game object in the scene by its tagname
    /// </summary>
    /// <returns> UI game object in scene. </returns>
    public GameObject GetLevelGameObjectByTag(UITag tagName)
    {
        return GameObject.FindGameObjectWithTag(tagName.ToString());
    }

    public GameObject GetLevelGameObjectByTag(GameObjectTag tagName)
    {
        return GameObject.FindGameObjectWithTag(tagName.ToString());
    }

    /// <summary>
    /// Gets the currently active scene and returns it as an ActiveGameScene or null if it was not any of the following
    /// </summary>
    /// <returns></returns>
    public ActiveGameScenes GetCurrentGameScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene.Equals(ActiveGameScenes.MainMenu.ToString()))
        {
            return ActiveGameScenes.MainMenu;
        }
        else if (currentScene.Equals(ActiveGameScenes.Level1.ToString()))
        {
            return ActiveGameScenes.Level1;
        }

        else if (currentScene.Equals(ActiveGameScenes.Level1_New.ToString()))
        {
            return ActiveGameScenes.Level1_New;
        }

        else if (currentScene.Equals(ActiveGameScenes.Level2.ToString()))
        {
            return ActiveGameScenes.Level2;
        }
        else if (currentScene.Equals(ActiveGameScenes.Level3.ToString()))
        {
            return ActiveGameScenes.Level3;
        }
        else if (currentScene.Equals(ActiveGameScenes.IntroScene.ToString()))
        {
            return ActiveGameScenes.IntroScene;
        }
        else if (currentScene.Equals(ActiveGameScenes.Credits.ToString()))
        {
            return ActiveGameScenes.Credits;
        }
        else
        {
            return ActiveGameScenes.CannotFind;
        }
    }

    #endregion

    #region Transitions/Scene Reloads

    public IEnumerator ContinueGame()
    {
        yield return new WaitForSeconds(DELAY_BETWEEN_SCENES);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public IEnumerator GoToMainMenu()
    {
        uiControl.ScreenFadeOut();
        yield return new WaitForSeconds(DELAY_BETWEEN_SCENES);

        //reset everything here when the player goes to the menu
        checkpointLocation = Vector3.zero;
        InventoryController.Instance.ResetInventoryItems();
        GameStatManager.Instance.ResetStats();
        ObjectiveController.Instance.ResetObjectives();
        SceneManager.LoadScene(ActiveGameScenes.MainMenu.ToString());
    }

    public IEnumerator GoToScene(ActiveGameScenes scene, GameState newState)
    {
        if (uiControl != null)
        {
            uiControl.ScreenFadeOut();
        }
        yield return new WaitForSeconds(DELAY_BETWEEN_SCENES);
        SceneManager.LoadScene(scene.ToString());
        canPress = true;
    }
    #endregion
}
