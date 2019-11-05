using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryPanelController : MonoBehaviour

{
    public List<Image> screenItems;

    public Image detailImage;
    public Text detailTitleText;
    public Text detailDescText;

    // Use this for initialization
    void Awake()
    {
        foreach (Text txt in GetComponentsInChildren<Text>())
        {
            if (txt.gameObject.name.ToLower() == "item title text")
            {
                detailTitleText = txt;
                continue;
            }
            if (txt.gameObject.name.ToLower() == "item details text")
            {
                detailTitleText = txt;
                continue;
            }
        }
        foreach (Image img in GetComponentsInChildren<Image>())
        {
            if (img.gameObject.name.ToLower() == "item details image")
            {
                detailImage = img;
                continue;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        if (InventoryController.Instance != null)
        {
            for (int i = 0; i < InventoryController.Instance.currentInventory.Count; i++)
            {
                InventoryItem itemSlot = screenItems[i].GetComponent<InventoryItem>();

                itemSlot.SetInventorySlotToItem(InventoryController.Instance.currentInventory[i]);
                if (i == 0 && GameController.Instance.currentGameOptions.inputSetting == InputType.Controller)
                {
                    StartCoroutine(DelaySelectFirst(itemSlot.GetComponent<Button>()));
                    itemSlot.SelectThisSlot(true);
                }
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
