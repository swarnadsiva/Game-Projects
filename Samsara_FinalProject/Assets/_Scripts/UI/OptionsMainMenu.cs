using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsMainMenu : MonoBehaviour
{

    public Button firstSelectedButton;
    public Button chkController;
    public Button chkKeyboard;
    public Button chkSFXOn;
    public Button chkMusicOn;
    public GameObject keyboardControls;
    public GameObject controllerControls;

    MainMenuController mainMenuCont;

    private void OnEnable()
    {
        if (mainMenuCont == null)
        {
            mainMenuCont = FindObjectOfType<MainMenuController>();
        }
        if (GameController.Instance != null)
        {
            if (GameController.Instance.currentGameOptions.inputSetting == InputType.Controller)
            {
                StartCoroutine(DelaySelectFirst(chkController));
            }

            switch (GameController.Instance.currentGameOptions.inputSetting)
            {
                case InputType.Controller:
                    SetGameOption(true, chkController);
                    SetGameOption(false, chkKeyboard);
                    keyboardControls.SetActive(false);
                    controllerControls.SetActive(true);
                    break;
                case InputType.Keyboard:
                    SetGameOption(true, chkKeyboard);
                    SetGameOption(false, chkController);
                    keyboardControls.SetActive(true);
                    controllerControls.SetActive(false);
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

    #region Options Button Click Events

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
    }

    public void ToggleMusic()
    {
       
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
        
        switch (GameController.Instance.currentGameOptions.inputSetting)
        {
            case InputType.Controller:
                GameController.Instance.currentGameOptions.inputSetting = InputType.Keyboard;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SetGameOption(true, chkKeyboard);
                SetGameOption(false, chkController);
                keyboardControls.SetActive(true);
                controllerControls.SetActive(false);
                break;
            case InputType.Keyboard:
                GameController.Instance.currentGameOptions.inputSetting = InputType.Controller;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                SetGameOption(false, chkKeyboard);
                SetGameOption(true, chkController);
                keyboardControls.SetActive(false);
                controllerControls.SetActive(true);
                break;
            default:
                break;
        }
    }



    #endregion
}
