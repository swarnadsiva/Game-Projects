using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stat : SelectableUI, ISelectHandler
{
    public Image statMask;
    public Button increaseButton;
    public Text maxedText;
     float increaseAmount = 65f;
    bool tempDisable = false;

    StatPanelController statPanelCont;
    int currentStatValue;
    StatType myStatType;
    public string MyDetailText { get; set; }
    bool m_canIncrease;
    AudioSource myAudio;

    public bool CanIncreaseThisStat
    {
        get
        {
            return m_canIncrease;
        }
        set
        {
            m_canIncrease = value;
            increaseButton.gameObject.SetActive(m_canIncrease);
            if (currentStatValue >= 4)
            {
                maxedText.gameObject.SetActive(true);
                increaseButton.gameObject.SetActive(false);
            }
        }
    }

    // Use this for initialization
    void Awake()
    {
        myAudio = FindObjectOfType<PauseScreenController>().GetComponent<AudioSource>();
        statPanelCont = GetComponentInParent<StatPanelController>();

        foreach (Image img in GetComponentsInChildren<Image>())
        {
            if (img.gameObject.name.ToLower().Contains("mask"))
            {
                statMask = img;
                continue;
            }

        }
        foreach (Text txt in GetComponentsInChildren<Text>())
        {
            if (txt.gameObject.name.ToLower().Contains("maxed"))
            {
                maxedText = txt;
                break;
            }
        }
        foreach (Button btn in GetComponentsInChildren<Button>())
        {
            if (btn.gameObject.name.ToLower().Contains("increase"))
            {
                increaseButton = btn;
                increaseButton.onClick.AddListener(OnIncreaseStatClick);
                break;
            }
        }

        maxedText.gameObject.SetActive(false);
        CanIncreaseThisStat = true;

        switch (gameObject.name.ToLower())
        {
            case "speed":
                myStatType = StatType.AttackSpeed;
                break;
            case "defense":
                myStatType = StatType.MovementSpeed;
                break;
            case "attack":
                myStatType = StatType.AttackDamage;
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CanIncreaseThisStat = (currentStatValue < 4 && GameStatManager.Instance.PlayerSkillPoints > 0) ? true : false;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (GameController.Instance.currentGameOptions.inputSetting == InputType.Keyboard)
        {
            if (GameController.Instance.currentGameOptions.sfxOn)
                myAudio.Play();
            statPanelCont.IsSelected = true;
            statPanelCont.statDetailsText.text = MyDetailText;
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (GameController.Instance.currentGameOptions.inputSetting == InputType.Keyboard)
        {
            statPanelCont.IsSelected = false;
            statPanelCont.statDetailsText.text = "";
        }
    }

    bool CheckCanIncrease()
    {
        return (currentStatValue < 4 && GameStatManager.Instance.PlayerSkillPoints > 0) ? true : false;
    }

    public void OnIncreaseStatClick()
    {
        CanIncreaseThisStat = CheckCanIncrease();
        if (CanIncreaseThisStat && !tempDisable)
        {
            tempDisable = true;
            StartCoroutine(ResetDisable());
            // increase this stat
            currentStatValue++;
            statMask.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentStatValue * increaseAmount);
            SetDetailText(false);
            statPanelCont.statDetailsText.text = MyDetailText;
            statPanelCont.IncreaseStat(myStatType);
        }
    }

    IEnumerator ResetDisable()
    {
        yield return new WaitForSeconds(0.2f);
        tempDisable = false;
    }

    void OnEnable()
    {
        if (GameController.Instance != null)
        {
            SetDetailText(true);
            statMask.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentStatValue * increaseAmount);


            if (GameController.Instance.currentGameOptions.inputSetting == InputType.Controller)
            {
                if (increaseButton.enabled)
                {
                    increaseButton.interactable = false;
                }
                GetComponentInParent<Button>().onClick.AddListener(OnIncreaseStatClick);
            }
        }
    }

    void SetDetailText(bool initializing)
    {
        switch (myStatType)
        {
            case StatType.MovementSpeed:
                if (initializing)
                    currentStatValue = GameStatManager.Instance.MovementSpeed;
                MyDetailText = "Kala's movement speed has been increased by " + (currentStatValue / 4f * 100f) + "%";
                break;
            case StatType.AttackDamage:
                if (initializing)
                    currentStatValue = GameStatManager.Instance.AttackDamage;
                MyDetailText = "Kala's attack damage amount has been increased by " + (currentStatValue / 4f * 100f) + "%";
                break;
            case StatType.AttackSpeed:
                if (initializing)
                    currentStatValue = GameStatManager.Instance.AttackSpeed;
                MyDetailText = "Kala's attack speed has been increased by " + (currentStatValue / 4f * 100f) + "%";
                break;
            default:
                break;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (GameController.Instance.currentGameOptions.inputSetting == InputType.Controller)
        {
            if (GameController.Instance.currentGameOptions.sfxOn)
            {
                myAudio.Play();
            }

               
            statPanelCont.IsSelected = true;
            statPanelCont.statDetailsText.text = MyDetailText;
        }
    }

    IEnumerator SelectThisStat()
    {
        if (statPanelCont.IsSelected)
        {
            statPanelCont.IsSelected = false;
            yield return new WaitForSeconds(0.5f);
        }
        statPanelCont.IsSelected = true;
        statPanelCont.statDetailsText.text = MyDetailText;
    }
}
