using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{

    public bool FadeIn = false;
    public Image img;
    public Text txt;
    public Button btn;

    public Color origColor;
    public Color transpColor;

    public const float speed = 2f;

    // Use this for initialization
    void Start()
    {
        FadeIn = false;
        btn = GetComponent<Button>();
        img = GetComponent<Image>();
        if (img != null)
        {
            origColor = new Color(img.color.r, img.color.g, img.color.b, 1);
            transpColor = new Color(img.color.r, img.color.g, img.color.b, 0);
        }
        else
        {
            txt = GetComponent<Text>();
            origColor = new Color(txt.color.r, txt.color.g, txt.color.b, 1);
            transpColor = new Color(txt.color.r, txt.color.g, txt.color.b, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (FadeIn)
        {
            print("in fade in");
            if (img != null)
            {
                GetComponent<Image>().color = Color.Lerp(img.color, origColor, Time.deltaTime * speed);
                if (img.color == origColor && btn != null)
                {
                    btn.interactable = true;
                }
            }

            if (txt != null)
            {
                print("in fade out");
                GetComponent<Text>().color = Color.Lerp(txt.color, origColor, Time.deltaTime * speed);
                if (txt.color == origColor && btn != null)
                {
                    btn.interactable = true;
                }
            }
        }

        else //fade out
        {
            if (img != null)
            {
                print("fadeout");

                GetComponent<Image>().color = Color.Lerp(img.color, transpColor, Time.deltaTime * speed);
                if (img.color == transpColor && btn != null)
                {
                    btn.interactable = false;
                }
            }

            if (txt != null)
            {
                GetComponent<Text>().color = Color.Lerp(txt.color, transpColor, Time.deltaTime * speed);
                if (txt.color == transpColor && btn != null)
                {
                    btn.interactable = false;
                }
            }
        }
    }

    private void OnDisable()
    {
        FadeIn = true;
    }
}
