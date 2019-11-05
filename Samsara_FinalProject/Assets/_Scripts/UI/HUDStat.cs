using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDStat : MonoBehaviour {
    public Image statMask;

    int currentStatValue;
    StatType myStatType;

    // Use this for initialization
    void Start()
    {
        foreach (Image img in GetComponentsInChildren<Image>())
        {
            if (img.gameObject.name.ToLower().Contains("mask"))
            {
                statMask = img;
                continue;
            }
        }
        
        switch (gameObject.name.ToLower())
        {
            case "attackspeed":
                myStatType = StatType.AttackSpeed;
                currentStatValue = GameStatManager.Instance.AttackSpeed;
                statMask.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentStatValue * 25f);
                break;
            case "movementspeed":
                myStatType = StatType.MovementSpeed;
                currentStatValue = GameStatManager.Instance.MovementSpeed;
                statMask.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentStatValue * 25f);
                break;
            case "attackdamage":
                myStatType = StatType.AttackDamage;
                currentStatValue = GameStatManager.Instance.AttackDamage;
                statMask.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentStatValue * 25f);
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        switch (myStatType)
        {
            case StatType.AttackSpeed:
                if (currentStatValue != GameStatManager.Instance.AttackSpeed)
                {
                    currentStatValue = GameStatManager.Instance.AttackSpeed;
                    statMask.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentStatValue * 25f);
                }
                break;
            case StatType.AttackDamage:
                if (currentStatValue != GameStatManager.Instance.AttackDamage)
                {
                    currentStatValue = GameStatManager.Instance.AttackDamage;
                    statMask.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentStatValue * 25f);
                }
                break;
            case StatType.MovementSpeed:
                if (currentStatValue != GameStatManager.Instance.MovementSpeed)
                {
                    currentStatValue = GameStatManager.Instance.MovementSpeed;
                    statMask.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentStatValue * 25f);
                }
                break;
            default:
                break;
        }

    }
}
