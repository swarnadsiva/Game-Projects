using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LogoScreenController : MonoBehaviour {

    public int timer = 0;
    public bool fadeIn = true;
    bool doOnce = false;
    public Image screenTransition;

	
	// Update is called once per frame
	void Update () {

        timer = (int) Time.time;

        if (timer == 2 && fadeIn)
        {
            fadeIn = false;
        }

        if (timer == 8 && !doOnce)
        {
            doOnce = true;
            StartCoroutine(GoToMainMenu());
        }

        if (fadeIn)
        {
            screenTransition.color = Color.Lerp(screenTransition.color, Color.clear, Time.deltaTime * 3f);
        }
        else
        {
            screenTransition.color = Color.Lerp(screenTransition.color, Color.black, Time.deltaTime * 2f);
        }
	}

    IEnumerator GoToMainMenu()
    { 
        SceneManager.LoadScene(ActiveGameScenes.Level1_New.ToString());
        yield return null;
    }
}
