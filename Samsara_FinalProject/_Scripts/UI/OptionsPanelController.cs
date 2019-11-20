using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsPanelController : MonoBehaviour
{
    public List<Sprite> resumeButtonSprites;
    public Image resumeButtonImage;
    public Button chkKeyboard;
    public Button chkController;
    public Button chkMusicOn;
    public Button chkSFXOn;
    public Image r1;
    public Image l1;

    public GameObject keyboardControls;
    public GameObject controllerControls;
    AudioSource myAudio;

    // Use this for initialization
    void Awake()
    {
        myAudio = FindObjectOfType<PauseScreenController>().GetComponent<AudioSource>();
        foreach (Image img in GetComponentsInChildren<Image>())
        {

            if (img.name.ToLower() == "controls")
            {
                keyboardControls = img.gameObject;
            }
            else if (img.name.ToLower() == "controller controls")
            {
                controllerControls = img.gameObject;
            }
        }

        foreach (Button btn in GetComponentsInChildren<Button>())
        {

            if (btn.name.ToLower() == "menu button")
            {
                btn.onClick.AddListener(MenuButtonClick);
            }
            else if (btn.name.ToLower() == "checkbox_keyboard")
            {
                chkKeyboard = btn;
                btn.onClick.AddListener(ToggleInput);
            }
            else if (btn.name.ToLower() == "checkbox_controller")
            {
                chkController = btn;
                btn.onClick.AddListener(ToggleInput);
            }
            else if (btn.name.ToLower() == "checkbox_music")
            {
                chkMusicOn = btn;
                btn.onClick.AddListener(ToggleMusic);
            }
            else if (btn.name.ToLower() == "checkbox_sfx")
            {
                chkSFXOn = btn;
                btn.onClick.AddListener(ToggleSFX);
            }
        }
    }

    public void MenuButtonClick()
    {
        StartCoroutine(GameController.Instance.GoToMainMenu());
    }

    public void ToggleSFX()
    {
        if (!GameController.Instance.currentGameOptions.sfxOn)
        {
            GameController.Instance.currentGameOptions.sfxOn = true;
            SetGameOption(true, chkSFXOn);
        }
        else
        {
            GameController.Instance.currentGameOptions.sfxOn = false;
            SetGameOption(false, chkSFXOn);
        }
        PlaySound();
    }

    public void ToggleMusic()
    {
        PlaySound();
        if (!GameController.Instance.currentGameOptions.musicOn)
        {
            GameController.Instance.currentGameOptions.musicOn = true;
            SoundManager.Instance.PlayMusic();
            SetGameOption(true, chkMusicOn);
        }
        else
        {
            GameController.Instance.currentGameOptions.musicOn = false;
            SoundManager.Instance.StopMusic();
            SetGameOption(false, chkMusicOn);
        }
    }

    public void ToggleInput()
    {
        PlaySound();
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                resumeButtonImage.sprite = resumeButtonSprites[(int)InputType.Keyboard];
                GameController.Instance.currentGameOptions.inputSetting = InputType.Keyboard;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SetGameOption(true, chkKeyboard);
                SetGameOption(false, chkController);
                keyboardControls.SetActive(true);
                controllerControls.SetActive(false);
                r1.enabled = false;
                l1.enabled = false;
                break;
            case InputType.Keyboard:
                resumeButtonImage.sprite = resumeButtonSprites[(int)InputType.Controller];
                GameController.Instance.currentGameOptions.inputSetting = InputType.Controller;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                SetGameOption(false, chkKeyboard);
                SetGameOption(true, chkController);
                keyboardControls.SetActive(false);
                controllerControls.SetActive(true);
                r1.enabled = true;
                l1.enabled = true;
                break;
            default:
                break;
        }
    }
    
    public void SetGameOption(bool boolVal, Button btn)
    {
        foreach (Image img in btn.GetComponentsInChildren<Image>())
        {
            if (img.name.ToLower() == "checkbox image")
            {
                img.enabled = boolVal;
                break;
            }
        }
    }

    private void OnEnable()
    {
        if (GameController.Instance != null)
        {
            switch (GameController.Instance.currentGameOptions.inputSetting)
            {
                case InputType.Controller:
                    resumeButtonImage.sprite = resumeButtonSprites[(int)InputType.Controller];
                    SetGameOption(true, chkController);
                    SetGameOption(false, chkKeyboard);
                    keyboardControls.SetActive(false);
                    controllerControls.SetActive(true);
                    StartCoroutine(DelaySelectFirst(chkController));
                    r1.enabled = true;
                    l1.enabled = true;
                    break;
                case InputType.Keyboard:
                    resumeButtonImage.sprite = resumeButtonSprites[(int)InputType.Keyboard];
                    SetGameOption(false, chkController);
                    SetGameOption(true, chkKeyboard);
                    keyboardControls.SetActive(true);
                    controllerControls.SetActive(false);
                    r1.enabled = false;
                    l1.enabled = false;
                    break;
                default:
                    break;
            }

            SetGameOption(GameController.Instance.currentGameOptions.musicOn, chkMusicOn);
            SetGameOption(GameController.Instance.currentGameOptions.sfxOn, chkSFXOn); 
        }
    }


    IEnumerator DelaySelectFirst(Button btn)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(btn.gameObject);
    }

    void PlaySound()
    {
        if (GameController.Instance.currentGameOptions.sfxOn && GameController.Instance.currentGameOptions.inputSetting == InputType.Keyboard)
        {
            myAudio.Stop();
            myAudio.Play();
        }
    }
}
