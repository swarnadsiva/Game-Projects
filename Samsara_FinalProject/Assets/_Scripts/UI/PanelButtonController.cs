using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PanelButtonController : SelectableUI
{

    public Image pausePanelScreen;
    public Image selectedStar;
    private Vector2 selectedStarPosition;

    private const float SPEED = 4f;
    public Image pausePanelButton;

    UIController uiControl;
    AudioSource myAudio;

    void Awake()
    {
        myAudio = FindObjectOfType<PauseScreenController>().GetComponent<AudioSource>();
        uiControl = GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>();
    }

    // Use this for initialization
    void Start()
    {

        pausePanelButton = GetComponent<Image>();

        if (gameObject.name.ToLower() == "objectives")
        {
            IsSelected = false;

            selectedStarPosition = new Vector2(85, 1);
        }
        else if (gameObject.name.ToLower() == "inventory")
        {
            IsSelected = false;

            selectedStarPosition = new Vector2(75, 1);
        }
        else if (gameObject.name.ToLower() == "stats")
        {
            IsSelected = false;

            selectedStarPosition = new Vector2(35, 1);
        }
        else if (gameObject.name.ToLower() == "options")
        {
            // by default options should be selected
            IsSelected = true;
            selectedStarPosition = new Vector2(50, 1);
        }

        uiControl.AddToPauseButtonList(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsSelected)
        {
            selectedStar.gameObject.transform.SetParent(transform);
            selectedStar.rectTransform.anchorMin = new Vector2(1, 0.5f);
            selectedStar.rectTransform.anchorMax = new Vector2(1, 0.5f);
            selectedStar.rectTransform.pivot = new Vector2(1, 0.5f);
            selectedStar.rectTransform.anchoredPosition = selectedStarPosition;
            pausePanelButton.color = Color.Lerp(pausePanelButton.color, Color.white, Time.deltaTime * SPEED);
            pausePanelScreen.gameObject.SetActive(true);
        }
        else
        {
            pausePanelButton.color = Color.Lerp(pausePanelButton.color, new Color(1, 1, 1, 0), Time.deltaTime * SPEED);
            pausePanelScreen.gameObject.SetActive(false);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        SelectThisPanel();
    }

    public void SelectThisPanel()
    {
        if (!IsSelected)
        {
            IsSelected = true;
            if (GameController.Instance.currentGameOptions.sfxOn)
            {
                if (myAudio != null)
                    myAudio.Play();
            }
                
        }
        if (uiControl != null)
            uiControl.SetPausePanelSelected(gameObject.name.ToLower());
       
    }

    public override void OnPointerExit(PointerEventData eventData)
    {

    }


}
