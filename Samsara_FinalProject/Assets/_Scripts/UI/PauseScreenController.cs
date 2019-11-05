using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreenController : MonoBehaviour
{
    // map 
    //      3             2         1         0
    // objectives    inventory     stats    options


    public enum Menu { Options, Stats, Inventory, Objectives };


    public int selectedIndex = (int)Menu.Options; // this will the options menu that is selected first

    PanelButtonController[] panelButtons = new PanelButtonController[4];
    AudioSource myAudio;

    // Use this for initialization
    void Awake()
    {
        myAudio = GetComponent<AudioSource>();
        // sort all the panel button controllers so we know where they are in panelButtons
        PanelButtonController[] temp = FindObjectsOfType<PanelButtonController>();
        foreach (PanelButtonController btnCont in temp)
        {
            if (btnCont.gameObject.name.ToLower() == "objectives")
            {
                panelButtons[(int)Menu.Objectives] = btnCont;
            }
            else if (btnCont.gameObject.name.ToLower() == "inventory")
            {
                panelButtons[(int)Menu.Inventory] = btnCont;
            }
            else if (btnCont.gameObject.name.ToLower() == "stats")
            {
                panelButtons[(int)Menu.Stats] = btnCont;
            }
            else if (btnCont.gameObject.name.ToLower() == "options")
            {
                panelButtons[(int)Menu.Options] = btnCont;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance != null)
        {
            if (GameController.Instance.currentGameOptions.inputSetting == InputType.Controller)
            {
                //check buttons
                if (selectedIndex > (int)Menu.Options && selectedIndex < (int)Menu.Objectives) // means we can move both directions
                {
                    if (InputController.CheckSelectLeftPauseMenu())
                    {
                        PlaySound();
                        selectedIndex++;
                        panelButtons[selectedIndex].SelectThisPanel();
                    }
                    if (InputController.CheckSelectRightPauseMenu())
                    {
                        PlaySound();
                        selectedIndex--;
                        panelButtons[selectedIndex].SelectThisPanel();
                    }
                }
                else if (selectedIndex == (int)Menu.Objectives) // means we can only move right (--)
                {
                    if (InputController.CheckSelectRightPauseMenu())
                    {
                        PlaySound();
                        selectedIndex--;
                        panelButtons[selectedIndex].SelectThisPanel();
                    }
                    if (InputController.CheckSelectLeftPauseMenu())
                    {
                        PlaySound();
                        selectedIndex = (int)Menu.Options; // go back to the options panel
                        panelButtons[selectedIndex].SelectThisPanel();
                    }
                }
                else if (selectedIndex == (int)Menu.Options) // means we can only move left (++)
                {
                    if (InputController.CheckSelectLeftPauseMenu())
                    {
                        PlaySound();
                        selectedIndex++;
                        panelButtons[selectedIndex].SelectThisPanel();
                    }
                    if (InputController.CheckSelectRightPauseMenu())
                    {
                        PlaySound();
                        selectedIndex = (int)Menu.Objectives; // go back to the objectives panel
                        panelButtons[selectedIndex].SelectThisPanel();
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        // always select the options on default of opening the pause screen
        selectedIndex = (int)Menu.Options;
        panelButtons[selectedIndex].SelectThisPanel();
    }

    public void PlaySound()
    {
        if (GameController.Instance.currentGameOptions.sfxOn)
            myAudio.Play();
    }
}
