using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsLoop : MonoBehaviour
{

    public Text myText;
    public Color origColor;
    private Color transparent;

    public float timer;
    public const int SWITCH_TIME = 4;
    public const float FADE_SPEED = 2f;

    public bool inFirstGroup;
    public bool showText;
    public bool beginLoop;
    public bool alreadyDone;

    // Use this for initialization
    void Start()
    {
        myText = GetComponent<Text>();
        if (myText != null)
        {
            origColor = new Color(myText.color.r, myText.color.g, myText.color.b, 1);
            transparent = new Color(myText.color.r, myText.color.g, myText.color.b, 0);

            myText.color = transparent;
        }
        timer = 0;
        beginLoop = true;
        showText = inFirstGroup ? false : true;
        alreadyDone = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (beginLoop)
        {
            timer += Time.deltaTime;
            if (Mathf.RoundToInt(timer) % SWITCH_TIME == 0 && !alreadyDone)
            {
                StartCoroutine(SwitchShowText());
                timer = 0;
            }

            if (showText)
            {
                myText.color = Color.Lerp(myText.color, origColor, Time.deltaTime * FADE_SPEED);
            }
            else
            {
                myText.color = Color.Lerp(myText.color, transparent, Time.deltaTime * FADE_SPEED);
            }
        }
        else
        {
            myText.color = Color.Lerp(myText.color, transparent, Time.deltaTime * FADE_SPEED * 2f);
        }


    }

    IEnumerator SwitchShowText()
    {
        alreadyDone = true;
        showText = !showText;
        yield return new WaitForSeconds(1f);
        alreadyDone = false;
    }

    private void OnEnable()
    {
        timer = 0;
        beginLoop = true;
    }

    private void OnDisable()
    {
        beginLoop = false;
    }
}
