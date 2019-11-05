using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonsMainMenu : MonoBehaviour {

    public Button play;
    public Button quit;
    public Button credits;
    public Button options;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnEnable()
    {
        if (GameController.Instance != null)
        {
            if (GameController.Instance.currentGameOptions.inputSetting == InputType.Controller)
            {
                StartCoroutine(DelaySelectFirst(play));
            }
        }
    }

    IEnumerator DelaySelectFirst(Button btn)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(btn.gameObject);
    }
}
