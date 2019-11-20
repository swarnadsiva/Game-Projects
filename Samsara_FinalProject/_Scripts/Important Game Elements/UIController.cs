using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SpriteKeyIndex { X, Circle, Q, Space, E };

/// <summary>
/// This class will handle all UI element transitions, changes, etc.
/// </summary>
public class UIController : MonoBehaviour
{
    public Color hurtColor;
    public float flashSpeed = 2f;

    //public bool CanPause { get; set; }
    public bool InMainOptions { get; set; }
    public bool InMainCredits { get; set; }

    #region Prefabs
    // need to stay public
    public List<Sprite> hudHurtImages;
    public List<GameObject> mainMenuUiPrefabs;
    public List<GameObject> gameUiPrefabs;
    public List<Sprite> energySprites;
    public List<Sprite> goldenorbSprites;
    public List<Sprite> interactionKeySprites;
    public GameObject minimapPrefab;

    public GameObject notificationPanel;
    public GameObject skipNotice;
    GameObject hudSkillPoints;

    #endregion

    #region Private Member Variables

    // change all these to private
    GameObject hudCanvas, pauseCanvas, gameOverCanvas, playerHealthCanvas, menuMainCanvas, godModeCanvas;
    GameObject miniMap;
    GameObject mainMenuBackButton;
    Text hudMessageText, hudInteractionText, hudLevelText, hudItemsCollectedText, hudDialogueText, hudDialogueSpeakerText;//, hudXPText;
    public Text hudSpeedNum, hudDefNum, hudAttackNum;
    Image hudHurtImage, hudDialogueImg, hudCollect1, hudCollect2, hudCollect3, hudEmpty1, hudEmpty2, hudEmpty3, hudInteractionKey, hudDialogueKey;
    //Slider hudXPSlider;
    Button gameOverContinueButton, gameOverMainMenuButton;
    Button chkKeyboard, chkController, chkMusicOn, chkSFXOn;
    Image pauseStatsPanel, pauseObjectivesPanel, pauseOptionsPanel, pauseInventoryPanel;
    GameObject keyboardControls, controllerControls, mainOptionsPanel, mainCreditsPanel, mainButtonsPanel;
    PlayerStatController playerStats;
    List<GameObject> pauseButtons = new List<GameObject>();
    Animator hudAnim, gameOverAnim, screenTransitionAnim, mainMenuAnim;
    InputController inputControl;

    Button creditsMenuButton;

    #endregion

    private void Start()
    {
        StartCoroutine(BeginUIEvents());
    }

    IEnumerator BeginUIEvents()
    {
        yield return new WaitForSeconds(0.2f);
        GameController.Instance.CanPause = false;
        screenTransitionAnim = GameObject.FindGameObjectWithTag("ScreenTransCanvas").GetComponent<Animator>();
        switch (GameController.Instance.GetCurrentGameScene())
        {
            case ActiveGameScenes.CannotFind:

                // do nothing but we should throw an error and quit
                //GameController.Instance.QuitGame();

                // for now, let this be our test scenes that need all elements
                LoadLevelElements();
                ToggleVisibility(pauseCanvas, false);
                ToggleVisibility(gameOverCanvas, false);
                StartCoroutine(ShowHUDInteractionText(false));
                ShowGodModeCanvas(false);

                break;
            case ActiveGameScenes.MainMenu:
                LoadMenuElements();

                break;
            case ActiveGameScenes.Level1_New:
                LoadLevelElements();
                ToggleVisibility(pauseCanvas, false);
                ToggleVisibility(gameOverCanvas, false);
                StartCoroutine(ShowHUDInteractionText(false));
                StartCoroutine(ShowBeginningLevelMessage("Ruins in the Jungle"));
                ShowGodModeCanvas(false);
                break;
            case ActiveGameScenes.Level3:
                LoadLevelElements();
                ToggleVisibility(pauseCanvas, false);
                ToggleVisibility(gameOverCanvas, false);
                StartCoroutine(ShowHUDInteractionText(false));
                StartCoroutine(ShowBeginningLevelMessage("Darkness Within"));
                ShowGodModeCanvas(false);
                break;
            default:
                break;
        }

        if (GameController.Instance.GetLevelGameObjectByTag(GameObjectTag.Player) != null)
        {
            playerStats = GameController.Instance.GetLevelGameObjectByTag(GameObjectTag.Player).GetComponent<PlayerStatController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerStats != null)
        {
            if (playerStats.IsDamaged)
            {

                hudHurtImage.color = hurtColor;
            }
            else
            {

                hudHurtImage.color = Color.Lerp(hudHurtImage.color, Color.clear, flashSpeed * Time.deltaTime);

            }
            playerStats.IsDamaged = false;

            if (GameStatManager.Instance.PlayerSkillPoints > 0 && !hudSkillPoints.activeInHierarchy)
            {
                hudSkillPoints.SetActive(true);
            }

            if (GameStatManager.Instance.PlayerSkillPoints <= 0 && hudSkillPoints.activeInHierarchy)
            {
                hudSkillPoints.SetActive(false);
            }
        }

        if (GameController.Instance.GetCurrentGameScene() == ActiveGameScenes.Credits && creditsMenuButton == null)
        {
            creditsMenuButton = GameObject.Find("Main menu button").GetComponent<Button>();
            creditsMenuButton.onClick.AddListener(CreditsMainMenu);
        }

        if (InventoryController.Instance.PlayerHasEnergyVessel && !GameController.Instance.CutScenePlaying && GameController.Instance.GetCurrentGameScene() == ActiveGameScenes.Level1_New)
        {
            hudEmpty1.enabled = true;
            hudEmpty2.enabled = true;
            hudEmpty3.enabled = true;
        }
       
    }


    public IEnumerator ShowBeginningLevelMessage(string levelName)
    {
        // yield return new WaitForSeconds(1f);
        ShowLevelName(false);
        ShowHUDMessage(true, levelName);
        yield return new WaitForSeconds(3f);
        ShowHUDMessage(false);
        yield return new WaitForSeconds(1f);
        GameController.Instance.CanPause = true;
        ShowLevelName(true, levelName);
        miniMap.SetActive(true);
    }

    #region Game/Menu Loading Functions

    void LoadMenuElements()
    {
        inputControl = FindObjectOfType<InputController>();

        CreateUIElements(ActiveGameScenes.MainMenu);

        menuMainCanvas = GameObject.FindGameObjectWithTag("MenuMainCanvas");

    }


    void CreateUIElements(ActiveGameScenes scene)
    {
        if (scene == ActiveGameScenes.MainMenu)
        {
            foreach (GameObject prefab in mainMenuUiPrefabs)
            {
                if (GameObject.FindGameObjectWithTag(prefab.tag) == null)
                {
                    Instantiate(prefab);
                }
            }
        }
        else // this is a game level scene
        {
            foreach (GameObject prefab in gameUiPrefabs)
            {
                if (GameObject.FindGameObjectWithTag(prefab.tag) == null)
                {
                    Instantiate(prefab);
                }
            }
            Instantiate(minimapPrefab);
        }
    }

    void LoadLevelElements()
    {
        CreateUIElements(ActiveGameScenes.Level1);
        //CreateUIElements(ActiveGameScenes.LevelX);

        miniMap = GameObject.FindGameObjectWithTag("Minimap");
        miniMap.SetActive(false);

        godModeCanvas = GameController.Instance.GetLevelGameObjectByTag(UITag.GodModeCanvas);

        LoadHUDItems();

        LoadGameOverItems();

        LoadPauseScreenItems();

        playerHealthCanvas = GameController.Instance.GetLevelGameObjectByTag(UITag.PlayerHealthCanvas);
        godModeCanvas = GameController.Instance.GetLevelGameObjectByTag(UITag.GodModeCanvas);

    }

    void LoadHUDItems()
    {
        // get references to canvas elements in scene
        hudCanvas = GameController.Instance.GetLevelGameObjectByTag(UITag.HUD);
        hudAnim = hudCanvas.GetComponent<Animator>();

        // set all HUD text elements according to their name
        foreach (Text textItem in hudCanvas.GetComponentsInChildren<Text>())
        {
            if (textItem.name.ToLower().Equals("items collected text"))
            {
                //print("set items collected text ui");
                hudItemsCollectedText = textItem;
            }
            else if (textItem.name.ToLower().Equals("message text"))
            {
                //print("set hud message text ui");
                hudMessageText = textItem;
            }
            else if (textItem.name.ToLower().Equals("skip text"))
            {
                skipNotice = textItem.gameObject;
                skipNotice.SetActive(false);
                GameController.Instance.skipNotice = skipNotice;
            }
            else if (textItem.name.ToLower().Equals("interaction text"))
            {
                hudInteractionText = textItem;
            }
            else if (textItem.name.ToLower().Equals("level name text"))
            {
                hudLevelText = textItem;
                hudLevelText.enabled = false;
            }
            else if (textItem.name.ToLower().Equals("dialogue text"))
            {
                hudDialogueText = textItem;
                hudDialogueText.enabled = false;
            }

            else if (textItem.name.ToLower().Equals("dialogue speaker"))
            {
                hudDialogueSpeakerText = textItem;
                hudDialogueSpeakerText.enabled = false;
            }
            else if (textItem.name.ToLower().Equals("speedstattext"))
            {
                hudSpeedNum = textItem;
            }
            else if (textItem.name.ToLower().Equals("defensestattext"))
            {
                hudDefNum = textItem;
            }
            else if (textItem.name.ToLower().Equals("attackstattext"))
            {
                hudAttackNum = textItem;
            }

        }

        foreach (Image img in hudCanvas.GetComponentsInChildren<Image>())
        {
            if (img.name.ToLower().Equals("hurt image"))
            {
                hudHurtImage = img;
            }
            else if (img.name.ToLower().Equals("dialogue background"))
            {
                hudDialogueImg = img;
                hudDialogueImg.enabled = false;
            }
            else if (img.gameObject.name.ToLower().Equals("collected item 1"))
            {
                hudCollect1 = img;
                hudCollect1.enabled = false;
            }
            else if (img.gameObject.name.ToLower().Equals("collected item 2"))
            {
                hudCollect2 = img;
                hudCollect2.enabled = false;
            }
            else if (img.gameObject.name.ToLower().Equals("collected item 3"))
            {
                hudCollect3 = img;
                hudCollect3.enabled = false;
            }
            else if (img.gameObject.name.ToLower().Equals("empty1"))
            {
                hudEmpty1 = img;
                hudEmpty1.enabled = false;
            }
            else if (img.gameObject.name.ToLower().Equals("empty2"))
            {
                hudEmpty2 = img;
                hudEmpty2.enabled = false;
            }
            else if (img.gameObject.name.ToLower().Equals("empty3"))
            {
                hudEmpty3 = img;
                hudEmpty3.enabled = false;
            }
            else if (img.name.ToLower().Equals("dialogue image"))
            {
                hudDialogueKey = img;
                hudDialogueKey.enabled = false;
            }
            else if (img.name.ToLower().Equals("interaction image"))
            {
                hudInteractionKey = img;
                hudInteractionKey.enabled = false;
            }
            else if (img.gameObject.name.ToLower().Equals("notificationpanel"))
            {
                notificationPanel = img.gameObject;
            }
            else if (img.gameObject.name.ToLower().Equals("skill points available"))
            {
                hudSkillPoints = img.gameObject;
            }
        }

        //hudXPSlider = hudCanvas.GetComponentInChildren<Slider>();
    }

    void LoadPauseScreenItems()
    {
        pauseCanvas = GameController.Instance.GetLevelGameObjectByTag(UITag.PauseCanvas);

        foreach (Image img in pauseCanvas.GetComponentsInChildren<Image>())
        {
            if (img.name.ToLower().Equals("stats panel"))
            {
                pauseStatsPanel = img;
            }
            else if (img.name.ToLower().Equals("objectives panel"))
            {
                pauseObjectivesPanel = img;
            }
            else if (img.name.ToLower().Equals("inventory panel"))
            {
                pauseInventoryPanel = img;
            }
            else if (img.name.ToLower().Equals("options panel"))
            {
                pauseOptionsPanel = img;
            }
        }
    }

    void LoadGameOverItems()
    {
        gameOverCanvas = GameController.Instance.GetLevelGameObjectByTag(UITag.GameOverCanvas);
        if (gameOverCanvas != null)
        {
            //print("gameoverCanvas not null");
        }
        gameOverAnim = gameOverCanvas.GetComponent<Animator>();

        // set all game over screen button elements
        foreach (Button buttonItem in gameOverCanvas.GetComponentsInChildren<Button>())
        {
            if (buttonItem.name.ToLower().Equals("continue button"))
            {
                //print("set game over button ui");
                gameOverContinueButton = buttonItem;
                gameOverContinueButton.onClick.AddListener(GameOverContinue);
            }
            else if (buttonItem.name.ToLower().Equals("main menu button"))
            {
                //print("set main mneu button ui");
                gameOverMainMenuButton = buttonItem;
                gameOverMainMenuButton.onClick.AddListener(GameOverMainMenu);
            }
        }
    }

    #endregion

    #region Public UI Visibility Functions

    public void ChangeHUDHurtImage()
    {
        int random = Random.Range(0, hudHurtImages.Count);
        hudHurtImage.sprite = hudHurtImages[random];
    }

    public IEnumerator ShowHUDInteractionText(bool doShow, string text = "", string command = "")
    {
        hudInteractionText.text = text;
        GameController.Instance.HudInteractionShowing = doShow;
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                if (command.ToLower() == "collect")
                {
                    hudInteractionKey.sprite = interactionKeySprites[(int)SpriteKeyIndex.X];
                    hudInteractionKey.preserveAspect = true;
                }
                else if (command.ToLower() == "drop")
                {
                    hudInteractionKey.sprite = interactionKeySprites[(int)SpriteKeyIndex.Circle];
                    hudInteractionKey.preserveAspect = true;
                }
                else if (command.ToLower() == "climb")
                {
                    hudInteractionKey.sprite = interactionKeySprites[(int)SpriteKeyIndex.X];
                    hudInteractionKey.preserveAspect = true;
                }
                break;
            case InputType.Keyboard:
                if (command.ToLower() == "collect")
                {
                    hudInteractionKey.sprite = interactionKeySprites[(int)SpriteKeyIndex.Q];
                    hudInteractionKey.preserveAspect = true;
                }
                else if (command.ToLower() == "drop")
                {
                    hudInteractionKey.sprite = interactionKeySprites[(int)SpriteKeyIndex.Space];
                    hudInteractionKey.preserveAspect = true;
                }
                else if (command.ToLower() == "climb")
                {
                    hudInteractionKey.sprite = interactionKeySprites[(int)SpriteKeyIndex.E];
                    hudInteractionKey.preserveAspect = true;
                }
                break;
            default:
                break;
        }
        StartCoroutine(FadeInteraction(doShow));
        hudInteractionText.enabled = true;
        hudInteractionKey.enabled = true;
        yield return null;
    }

    public IEnumerator FadeInteraction(bool doShow)
    {
        if (doShow)
        {

            for (int i = 0; i < 20; i++)
            {
                hudInteractionText.color = new Color(hudInteractionText.color.r, hudInteractionText.color.g, hudInteractionText.color.b, i / 20f);
                hudInteractionKey.color = new Color(hudInteractionKey.color.r, hudInteractionKey.color.g, hudInteractionKey.color.b, i / 20f);
                yield return null;
            }

        }
        else
        {
            for (int i = 20; i >= 0; i--)
            {
                hudInteractionText.color = new Color(hudInteractionText.color.r, hudInteractionText.color.g, hudInteractionText.color.b, i / 20f);
                hudInteractionKey.color = new Color(hudInteractionKey.color.r, hudInteractionKey.color.g, hudInteractionKey.color.b, i / 20f);
                yield return null;
            }
        }
    }

    string[] tempDialogue;
    int tempDialogueIndex;
    string currentSpeaker;
    public IEnumerator ShowDialogueText(bool doShow, string[] text = null, string speaker = "Kala")
    {
        currentSpeaker = speaker;
        if (text != null)
        {
            tempDialogue = null;
            tempDialogueIndex = 0;
            tempDialogue = text;

            ChangeDialogueText(tempDialogue[tempDialogueIndex]);
            tempDialogueIndex++; //increment b/c we have just shown the first one

            FindObjectOfType<UIDialogueController>().isShowing = doShow;
        }
        hudDialogueSpeakerText.text = speaker;

        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                hudDialogueKey.sprite = interactionKeySprites[(int)SpriteKeyIndex.X];
                hudDialogueKey.preserveAspect = true;
                break;
            case InputType.Keyboard:
                hudDialogueKey.sprite = interactionKeySprites[(int)SpriteKeyIndex.Space];
                hudDialogueKey.preserveAspect = true;
                break;
            default:
                break;
        }

        StartCoroutine(FadeDialogue(doShow));
        hudDialogueSpeakerText.enabled = true;
        hudDialogueKey.enabled = true;
        hudDialogueImg.enabled = true;
        hudDialogueText.enabled = true;


        yield return null;
    }

    public void ChangeDialogueText(string text)
    {
        hudDialogueText.text = text;
    }

    public void ShowNextDialogue()
    {
        if (tempDialogueIndex < tempDialogue.Length)
        {
            ChangeDialogueText(tempDialogue[tempDialogueIndex]);
            tempDialogueIndex++;
        }
        else
        {
            StartCoroutine(FindObjectOfType<UIDialogueController>().DialogueComplete());
            StartCoroutine(ShowDialogueText(false, null, currentSpeaker));
        }
    }

    public IEnumerator FadeDialogue(bool doShow)
    {
        if (doShow)
        {

            for (int i = 0; i < 20; i++)
            {
                hudDialogueSpeakerText.color = new Color(hudDialogueSpeakerText.color.r, hudDialogueSpeakerText.color.g, hudDialogueSpeakerText.color.b, i / 20f);
                hudDialogueKey.color = new Color(hudDialogueKey.color.r, hudDialogueKey.color.g, hudDialogueKey.color.b, i / 20f);
                hudDialogueText.color = new Color(hudDialogueText.color.r, hudDialogueText.color.g, hudDialogueText.color.b, i / 20f);
                hudDialogueImg.color = new Color(hudDialogueImg.color.r, hudDialogueImg.color.g, hudDialogueImg.color.b, i / 40f);
                yield return null;
            }

        }
        else
        {
            for (int i = 20; i >= 0; i--)
            {
                hudDialogueSpeakerText.color = new Color(hudDialogueSpeakerText.color.r, hudDialogueSpeakerText.color.g, hudDialogueSpeakerText.color.b, i / 20f);
                hudDialogueKey.color = new Color(hudDialogueKey.color.r, hudDialogueKey.color.g, hudDialogueKey.color.b, i / 20f);
                hudDialogueText.color = new Color(hudDialogueText.color.r, hudDialogueText.color.g, hudDialogueText.color.b, i / 20f);
                hudDialogueImg.color = new Color(hudDialogueImg.color.r, hudDialogueImg.color.g, hudDialogueImg.color.b, i / 40f);
                yield return null;
            }

        }
    }

    public void ShowGodModeCanvas(bool doShow)
    {
        godModeCanvas.SetActive(doShow);
    }

    public void SetHUDHurtColor(Color col)
    {
        hudHurtImage.color = col;
    }

    public Color GetHUDHurtColor()
    {
        return hudHurtImage.color;
    }

    public void ShowLevelName(bool doShow, string levelName = "")
    {
        hudLevelText.enabled = doShow;
        hudLevelText.text = levelName;
    }

    public void ScreenFadeOut()
    {
        StartAnimation(screenTransitionAnim, "FadeOut");
    }

    public void ScreenFadeIn()
    {
        StartAnimation(screenTransitionAnim, "FadeIn");
    }

    public void ShowHUDMessage(bool doShow, string message = "")
    {
        if (doShow)
        {
            hudMessageText.text = message;
            hudAnim.SetTrigger("Show");
        }
        else
        {
            hudAnim.SetTrigger("Hide");
        }
    }

    public void UpdateEnergyOrbsCollected(int numberCollected)
    {
        if (numberCollected == 1)
        {
            hudCollect1.enabled = true;
            hudCollect2.enabled = false;
            hudCollect3.enabled = false;
        }
        else if (numberCollected == 2)
        {
            hudCollect2.enabled = true;
            hudCollect1.enabled = true;
            hudCollect3.enabled = false;

        }
        else if (numberCollected == 3)
        {
            hudCollect3.enabled = true;
            hudCollect2.enabled = true;
            hudCollect1.enabled = true;
        }
        else
        {
            hudCollect1.enabled = false;
            hudCollect2.enabled = false;
            hudCollect3.enabled = false;
        }
    }

    public void ShowGameOver()
    {
        gameOverCanvas.SetActive(true);
        StartAnimation(gameOverAnim, GameOverAnimationTriggers.Show.ToString());
    }

    public void ShowPauseOverlay(bool doShow)
    {
        ShowPausePanels(true, false, false); //default show options?
        pauseCanvas.SetActive(doShow);
    }

    public void ShowPausePanels(bool showOptions, bool showInventory, bool showStats)
    {
        pauseOptionsPanel.gameObject.SetActive(showOptions);
        pauseInventoryPanel.gameObject.SetActive(showInventory);
        pauseStatsPanel.gameObject.SetActive(showStats);
    }

    public void SetHUDCanvasActive(bool isActive)
    {
        hudCanvas.SetActive(isActive);
    }

    private void ToggleVisibility(GameObject canvas, bool isVisible)
    {
        if (canvas != null)
        {
            canvas.SetActive(isVisible);
        }
    }

    public static void StartAnimation(Animator anim, string trigger)
    {
        anim.SetTrigger(trigger);
    }

    #endregion

    #region Pause Menu Functions

    public void AddToPauseButtonList(GameObject btn)
    {
        if (!pauseButtons.Contains(btn))
        {
            pauseButtons.Add(btn);
        }
    }

    public void SetPausePanelSelected(string buttonName)
    {

        // hide all other panels except for this one
        foreach (GameObject btn in pauseButtons)
        {
            if (btn.name.ToLower() == buttonName)
            {
                btn.GetComponent<PanelButtonController>().IsSelected = true;
            }
            else
            {
                btn.GetComponent<PanelButtonController>().IsSelected = false;
            }
        }
    }

    #endregion

    public void GameOverContinue()
    {

        gameOverContinueButton.interactable = false;
        gameOverMainMenuButton.interactable = false;
        StartAnimation(gameOverAnim, "Hide");
        StartCoroutine(GameController.Instance.ContinueGame());
    }

    public void GameOverMainMenu()
    {

        gameOverContinueButton.interactable = false;
        gameOverMainMenuButton.interactable = false;
        StartAnimation(gameOverAnim, "Hide");
        StartCoroutine(GameController.Instance.GoToMainMenu());
    }

    public void CreditsMainMenu()
    {
        print("going to main menu");
        StartCoroutine(GameController.Instance.GoToMainMenu());
    }
}
