using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{

    public Text introText;
    public ParticleSystem particles;
    public float timer = 0;
    public float SWITCH_TIME = 10f;
    public string[] introDialogueText;
    int index = 0;
    bool doOnce = false;

    // Use this for initialization
    void Start()
    {
        introText.text = introDialogueText[index];
        StartCoroutine(FadeDialogue(true));
        index++;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= SWITCH_TIME)
        {
            timer = 0f;
            StartCoroutine(ShowNextDialogue());
        }

        if (index == introDialogueText.Length && !doOnce)
        {
            doOnce = true;
            StartCoroutine(PlayGame());
        }
    }

    public IEnumerator FadeDialogue(bool doShow)
    {
        if (doShow)
        {
            for (int i = 0; i < 20; i++)
            {
                introText.color = new Color(introText.color.r, introText.color.g, introText.color.b, i / 20f);
                yield return null;
            }
        }
        else
        {
            for (int i = 20; i >= 0; i--)
            {
                introText.color = new Color(introText.color.r, introText.color.g, introText.color.b, i / 20f);
                yield return null;
            }
        }
    }

    public IEnumerator ShowNextDialogue()
    {
        if (index < introDialogueText.Length)
        {
            StartCoroutine(FadeDialogue(false));
            yield return new WaitForSeconds(2f);
            introText.text = introDialogueText[index];
            StartCoroutine(FadeDialogue(true));
            if (index == 3)
            {
                particles.Stop();
            }
            index++;
        }
        else
        {
            StartCoroutine(FadeDialogue(false));
        }
    }

    public IEnumerator PlayGame()
    {
        print("in play game");
        yield return new WaitForSeconds(10f);
        print("going to new scene");
        StartCoroutine(GameController.Instance.GoToScene(ActiveGameScenes.Level1_New, GameState.Playing));
    }
}
