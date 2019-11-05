using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    public GameObject optionsCanvas;
    public GameObject mainButtonsCanvas;
    public GameObject creditsCanvas;

    AudioSource myAudio;

    // Use this for initialization
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
        BackButtonClick();
    }

    private void Update()
    {
        if (InputController.CheckMenuGoBack() && (creditsCanvas.activeInHierarchy || optionsCanvas.activeInHierarchy))
        {
            print("back clicked");
            BackButtonClick();
        }
    }

    #region MainMenu Button Click Events 

    public void PlayButtonClick()
    {
        StartCoroutine(GameController.Instance.GoToScene(ActiveGameScenes.IntroScene, GameState.MainMenu));
    }

    public void QuitButtonClick()
    {
        GameController.Instance.QuitGame();
    }

    public void CreditsButtonClick()
    {
        mainButtonsCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
        creditsCanvas.SetActive(true);

        if (GameController.Instance.currentGameOptions.inputSetting == InputType.Controller)
        {
            foreach (Button item in creditsCanvas.GetComponentsInChildren<Button>())
            {
                if (item.gameObject.name.ToLower().Contains("back"))
                {
                    item.Select();
                }
            }
        }
    }

    public void OptionsButtonClick()
    {
        mainButtonsCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
        creditsCanvas.SetActive(false);
    }

    public void BackButtonClick()
    {
        mainButtonsCanvas.SetActive(true);
        optionsCanvas.SetActive(false);
        creditsCanvas.SetActive(false);
    }

    public void PlayClickSound()
    {
        if (GameController.Instance.currentGameOptions.sfxOn)
        {
            myAudio.Play();
        }
    }

    // for main menu
    IEnumerator SetPanelActive(GameObject pnl, bool isActive)
    {
        if (isActive)
        {
            pnl.SetActive(isActive);
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(1f);
            pnl.SetActive(isActive);
        }
    }

    #endregion
}
