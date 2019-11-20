using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum StatType { AttackSpeed, AttackDamage, MovementSpeed };

public class StatPanelController : MonoBehaviour
{

    //public Text currentXP;
    public Text numSkillPoints;
    public Text statDetailsText;

    public Color normalColor;
    public Color fadeColor;

    public bool IsSelected { get; set; }

    // Use this for initialization
    void Awake()
    {
        IsSelected = false;
        foreach (Text txt in GetComponentsInChildren<Text>())
        {
            //if (txt.gameObject.name.ToLower().Contains("purity of heart"))
            //{
            //    currentXP = txt;
            //    continue;
            //}
            if (txt.gameObject.name.ToLower().Contains("skill points"))
            {
                numSkillPoints = txt;
                continue;
            }
            if (txt.gameObject.name.ToLower().Contains("stat details"))
            {
                statDetailsText = txt;
                continue;
            }
        }
        normalColor = new Color(statDetailsText.color.r, statDetailsText.color.g, statDetailsText.color.b, 1);
        fadeColor = new Color(statDetailsText.color.r, statDetailsText.color.g, statDetailsText.color.b, 0);
    }

    private void Update()
    {
        if (IsSelected)
        {
            statDetailsText.color = Color.Lerp(statDetailsText.color, normalColor, Time.deltaTime * SelectableUI.FADE_SPEED);
        }
        else
        {
            statDetailsText.color = Color.Lerp(statDetailsText.color, fadeColor, Time.deltaTime * SelectableUI.FADE_SPEED);
        }
    }

    private void OnEnable()
    {
        if (GameController.Instance != null)
        {
            //currentXP.text = "Purity of Heart [XP] " + GameController.Instance.PlayerExperience.ToString() + "/100";
            numSkillPoints.text = "Available Skill Points: " + GameStatManager.Instance.PlayerSkillPoints.ToString();
            statDetailsText.text = "";
            statDetailsText.color = Color.clear;

            if (GameController.Instance.currentGameOptions.inputSetting == InputType.Controller)
            {
                Stat[] temp = FindObjectsOfType<Stat>();
                StartCoroutine(DelaySelectFirst(temp[1].GetComponent<Button>()));
            }
        }
    }

    public void IncreaseStat(StatType statToIncrease)
    {
        switch (statToIncrease)
        {
            case StatType.MovementSpeed:
                GameStatManager.Instance.MovementSpeed++;
                break;
            case StatType.AttackDamage:
                GameStatManager.Instance.AttackDamage++;
                break;
            case StatType.AttackSpeed:
                GameStatManager.Instance.AttackSpeed++;
                break;
            default:
                break;
        }
        GameStatManager.Instance.PlayerSkillPoints--;
        numSkillPoints.text = "Available Skill Points: " + GameStatManager.Instance.PlayerSkillPoints.ToString();
    }

    IEnumerator DelaySelectFirst(Button btn)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(btn.gameObject);
    }
}
