using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPointsColorLerp : MonoBehaviour
{

    public Text myText;
    public Color col1;
    public Color col2;

    // Use this for initialization
    void Start()
    {
        myText = GetComponent<Text>();
        col1 = new Color(myText.color.r, myText.color.g, myText.color.b);
        col2 = new Color(0, 0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (myText.color != col1)
        {
            myText.color = Color.Lerp(myText.color, col1, Time.deltaTime * 2f);
        }
        else
        {
            myText.color = Color.Lerp(myText.color, col2, Time.deltaTime * 2f);
        }

    }

}
