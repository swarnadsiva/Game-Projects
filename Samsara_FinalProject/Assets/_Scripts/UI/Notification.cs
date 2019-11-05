using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{

    public Sprite[] buttonSprites;
    public string MyNotificationText;
    public float MaxYPos;
    public bool ShowOptionsButton;

    bool FadeIn;
    bool MoveUp;
    bool doOnce;
    public float timer = 0f;
    float timeOnScreen = 5f;

    RectTransform myRect;

    public Image background;
    public Image optionsButton;
    public Text notificationText;

    private void OnEnable()
    {
        if (GameController.Instance.currentGameOptions.inputSetting == InputType.Controller)
        {
            optionsButton.sprite = buttonSprites[(int)InputType.Controller];
        }
        else
        {
            optionsButton.sprite = buttonSprites[(int)InputType.Keyboard];
        }
        //maxYpos should have been set
        FadeIn = true;
        MoveUp = true;

        //start pivot at the bottom
        myRect = GetComponent<RectTransform>();
        myRect.anchorMin = new Vector2(0.5f, 0f);
        myRect.anchorMax = new Vector2(0.5f, 0f);
        myRect.pivot = new Vector2(0.5f, 0f);
        myRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 380);
        myRect.anchoredPosition = new Vector2(0f, 0f);

        background.color = new Color(background.color.r, background.color.g, background.color.b, 0);
        optionsButton.color = new Color(optionsButton.color.r, optionsButton.color.g, optionsButton.color.b, 0);
        notificationText.color = new Color(notificationText.color.r, notificationText.color.g, notificationText.color.b, 0);

    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.currentGameOptions.inputSetting == InputType.Controller && optionsButton.sprite != buttonSprites[(int)InputType.Controller])
        {
            optionsButton.sprite = buttonSprites[(int)InputType.Controller];
        }
        else if(GameController.Instance.currentGameOptions.inputSetting == InputType.Keyboard && optionsButton.sprite != buttonSprites[(int)InputType.Keyboard])
        {
            optionsButton.sprite = buttonSprites[(int)InputType.Keyboard];
        }

        timer += Time.deltaTime;
        if (notificationText.text != MyNotificationText)
        {
            notificationText.text = MyNotificationText;
        }
        if (ShowOptionsButton && !optionsButton.enabled)
        {
            optionsButton.enabled = true;
        }
        else if (!ShowOptionsButton && optionsButton.enabled)
        {
            optionsButton.enabled = false;
        }

        if (MoveUp)
        {
            myRect.anchoredPosition = Vector3.Lerp(myRect.anchoredPosition, new Vector2(0f, myRect.anchoredPosition.y + 120f), Time.deltaTime * 2f);
            if (myRect.anchoredPosition.y >= MaxYPos)
            {
                MoveUp = false;
            }
        }
        else
        {
            if (FadeIn && timer >= timeOnScreen)
                FadeIn = false;
        }

        if (FadeIn)
        {
            background.color = Color.Lerp(background.color, new Color(background.color.r, background.color.g, background.color.b, background.color.a + 0.05f), Time.deltaTime * 4f);
            optionsButton.color = Color.Lerp(optionsButton.color, new Color(optionsButton.color.r, optionsButton.color.g, optionsButton.color.b, optionsButton.color.a + 0.5f), Time.deltaTime * 4f);
            notificationText.color = Color.Lerp(notificationText.color, new Color(notificationText.color.r, notificationText.color.g, notificationText.color.b, notificationText.color.a + 0.5f), Time.deltaTime * 4f);
        }
        else
        {
            background.color = Color.Lerp(background.color, Color.clear, Time.deltaTime * 4f);
            optionsButton.color = Color.Lerp(optionsButton.color, Color.clear, Time.deltaTime * 4f);
            notificationText.color = Color.Lerp(notificationText.color, Color.clear, Time.deltaTime * 4f);
        }

        if (!FadeIn && !MoveUp && !doOnce)
        {
            doOnce = true;
            StartCoroutine(DestroySelf());
        }
    }

    IEnumerator DestroySelf()
    {
        FindObjectOfType<NotificationController>().ResetMaxPosY();
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
