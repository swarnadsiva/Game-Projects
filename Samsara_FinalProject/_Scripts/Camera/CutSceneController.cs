using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneController : MonoBehaviour
{

    public GameObject firstCameraPoint;
    public QuickCutsceneController myCutScene;
    public float startDelay;
    GameObject player;

    GameObject minimapUI;
    GameObject levelTextUI;

    bool uiEnabled = true;

    // Use this for initialization
    void Start()
    {
        minimapUI = GameObject.Find("Map back mask");
        levelTextUI = GameObject.Find("Level Name Text");
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (minimapUI == null || levelTextUI == null)
        {
            minimapUI = GameObject.Find("Map back mask");
            levelTextUI = GameObject.Find("Level Name Text");
        }
        else if (minimapUI != null && levelTextUI != null)
        {
            if (myCutScene.playingCutscene && uiEnabled)
            {
                uiEnabled = false;
                minimapUI.SetActive(false);
                levelTextUI.SetActive(false);
            }
            else if (!myCutScene.playingCutscene && !uiEnabled)
            {
                uiEnabled = true;
                minimapUI.SetActive(true);
                levelTextUI.SetActive(true);
            }
        }
    }

    public void StartCutScene()
    {
        firstCameraPoint.transform.position = Camera.main.transform.position;
        myCutScene.ActivateCutscene();
    }

    public IEnumerator StartCutSceneWithFade()
    {

        UIController uiControl = FindObjectOfType<UIController>();
        uiControl.ScreenFadeOut();
        firstCameraPoint.transform.position = Camera.main.transform.position;
        yield return new WaitForSeconds(startDelay);
        uiControl.ScreenFadeIn();
        myCutScene.ActivateCutscene();
    }
}
