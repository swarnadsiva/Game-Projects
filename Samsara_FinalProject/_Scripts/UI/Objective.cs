using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Objective : SelectableUI, ISelectHandler, IMoveHandler
{

    public Color unselectedColor;
    public Image selectedImage;


    Color selectedColor;
    AudioSource myAudio;

    #region Accessible Properties

    private string m_text;
    public string TitleText
    {
        get
        {
            return m_text;
        }
        set
        {
            m_text = value;
            GetComponentInChildren<Text>().text = m_text;
        }
    }

    public string Description { get; set; }
    public Text DetailSection { get; set; }

    #endregion


    // Use this for initialization
    void Awake()
    {
        selectedColor = new Color(gameObject.GetComponentInChildren<Text>().color.r, gameObject.GetComponentInChildren<Text>().color.g, gameObject.GetComponentInChildren<Text>().color.b);
        IsSelected = false;
        myAudio = FindObjectOfType<PauseScreenController>().GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsSelected)
        {
            GetComponentInChildren<Text>().color = Color.Lerp(GetComponentInChildren<Text>().color, selectedColor, Time.deltaTime * FADE_SPEED);
        }
        else
        {
            GetComponentInChildren<Text>().color = Color.Lerp(GetComponentInChildren<Text>().color, unselectedColor, Time.deltaTime * FADE_SPEED);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (GameController.Instance.currentGameOptions.inputSetting == InputType.Keyboard)
        {
            SelectThisObjective(true);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (GameController.Instance.currentGameOptions.inputSetting == InputType.Keyboard)
        {
            SelectThisObjective(false);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        SelectThisObjective(true);
    }

    public void OnMove(AxisEventData eventData)
    {
        SelectThisObjective(false);
    }

    void SelectThisObjective(bool doSelect)
    {
        if (doSelect)
        {
            if (GameController.Instance.currentGameOptions.sfxOn)
                myAudio.Play();
            IsSelected = true;
            DetailSection.text = Description;
        }
        else
        {
            IsSelected = false;
        }
    }
}
