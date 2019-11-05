using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InventoryItem : SelectableUI, ISelectHandler, IMoveHandler
{

    //public Image hoverImage;
    public Image myItemImage;

    public Text detailBodyText;
    public Text detailTitleText;
    public Image detailImage;

    public bool HasItem { get; set; }
    public string MyName { get; set; }
    public string MyDescription { get; set; }

    private const float SPEED = 4f;
    AudioSource myAudio;

    // Use this for initialization
    void Awake()
    {
        IsSelected = false;
        detailImage.color = Color.clear;

        myItemImage.color = Color.clear;
        HasItem = false;
        myAudio = FindObjectOfType<PauseScreenController>().GetComponent<AudioSource>();
        //GetComponent<Button>().interactable = false;
    }

    public void SetInventorySlotToItem(Item newItem)
    {
        HasItem = true;
        GetComponent<Button>().interactable = true;
        // set sprite image
        myItemImage.sprite = newItem.IconImage;

        // make image visible
        myItemImage.color = Color.white;

        //set properties
        MyName = newItem.Name;
        MyDescription = newItem.Description;
    }

    public void SelectThisSlot(bool doSelect)
    {
        if (doSelect)
        {
            IsSelected = true;
            detailTitleText.text = MyName;
            detailBodyText.text = MyDescription;
            detailImage.sprite = myItemImage.sprite;
            detailImage.preserveAspect = true;
        }
        else
        {
            IsSelected = false;
            detailBodyText.color = Color.clear;
            detailTitleText.color = Color.clear;
            detailImage.color = Color.clear;
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (GameController.Instance.currentGameOptions.inputSetting == InputType.Keyboard)
        {
            if (HasItem && !IsSelected)
            {
                if (GameController.Instance.currentGameOptions.sfxOn)
                myAudio.Play();
                SelectThisSlot(true);
            }
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (GameController.Instance.currentGameOptions.inputSetting == InputType.Keyboard)
        {
            if (HasItem)
            {
                SelectThisSlot(false);
            }
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (HasItem && !IsSelected)
        {
            if (GameController.Instance.currentGameOptions.sfxOn)
                myAudio.Play();
            SelectThisSlot(true);
        }
    }

    public void OnMove(AxisEventData eventData)
    {
        if (GameController.Instance.currentGameOptions.inputSetting == InputType.Controller)
        {
            if (HasItem)
            {
                SelectThisSlot(false);
            }
        }
        else if (GameController.Instance.currentGameOptions.inputSetting == InputType.Keyboard)
        {
            if (HasItem)
            {
                SelectThisSlot(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsSelected && HasItem)
        {
            detailImage.color = Color.Lerp(detailImage.color, Color.white, Time.deltaTime * SPEED);
            detailTitleText.color = Color.Lerp(detailTitleText.color, Color.white, Time.deltaTime * SPEED);
            detailBodyText.color = Color.Lerp(detailBodyText.color, Color.white, Time.deltaTime * SPEED);
        }
    }
}
