using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPoints : MonoBehaviour {

    public List<Sprite> buttonSprites;
    public Image buttonImage;

    private void OnEnable()
    {
        if (GameController.Instance != null)
        {
            if (GameController.Instance.currentGameOptions.inputSetting == InputType.Controller)
            {
                buttonImage.sprite = buttonSprites[(int)InputType.Controller];
            }
            else
            {
                buttonImage.sprite = buttonSprites[(int)InputType.Keyboard];
            }
        }
    }

    private void Update()
    {
        if (GameController.Instance.currentGameOptions.inputSetting == InputType.Controller && buttonImage.sprite != buttonSprites[(int)InputType.Controller])
        {
            buttonImage.sprite = buttonSprites[(int)InputType.Controller];
        }
        else if (GameController.Instance.currentGameOptions.inputSetting == InputType.Keyboard && buttonImage.sprite != buttonSprites[(int)InputType.Keyboard])
        {
            buttonImage.sprite = buttonSprites[(int)InputType.Keyboard];
        }
    }
}
