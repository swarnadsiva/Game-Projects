using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDSkip : MonoBehaviour {

    public List<Sprite> buttonSprites;
    public Image buttonImage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
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
