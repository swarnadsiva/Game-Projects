using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour {

    public Button continueButton;
    public Button mainMenuButton;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnEnable()
    {
        if (GameController.Instance.currentGameOptions.inputSetting == InputType.Controller)
        {
            StartCoroutine(DelaySelectFirst(continueButton));
        }
    }

    IEnumerator DelaySelectFirst(Button btn)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(btn.gameObject);
    }
}
