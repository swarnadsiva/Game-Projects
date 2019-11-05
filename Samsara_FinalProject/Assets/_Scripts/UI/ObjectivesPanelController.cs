using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectivesPanelController : MonoBehaviour
{

   // public GameObject titleGameObject;
   public List<string> objectivesCollection;

    public GameObject objectiveTitlePrefab;

    //references to objective panel components
    public GameObject detailSectionText;
    public GameObject titleSection;
    

    // Use this for initialization
    void Awake()
    {

        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.name.ToLower() == "title section")
            {
                titleSection = child.gameObject;
                continue;
            }

            if (child.gameObject.name.ToLower() == "objectives detail text")
            {
                detailSectionText = child.gameObject;
                continue;
            }
        }
    }

    IEnumerator LoadCurrentObjectives()
    {
        //ensure we don't duplicate/create on top of each other
        objectivesCollection.Clear();

        if (GameController.Instance != null)
        {
            float posY = 0;

            // create a text object and place it in the title section for each objective
            foreach (KeyValuePair<ObjectiveTitle, GameObjective> entry in ObjectiveController.Instance.currentObjectives)
            {
                if (!objectivesCollection.Contains(entry.Key.ToString()))
                {
                    objectivesCollection.Add(entry.Key.ToString());


                    GameObject temp = Instantiate(objectiveTitlePrefab);
                    temp.transform.SetParent(titleSection.transform);

                    RectTransform tempRect = temp.GetComponent<RectTransform>();
                    tempRect.anchorMin = new Vector2(0.5f, 1f);
                    tempRect.anchorMax = new Vector2(0.5f, 1f);
                    tempRect.pivot = new Vector2(0.5f, 1f);
                    tempRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
                    tempRect.anchoredPosition = new Vector2(0f, posY);

                    temp.GetComponentInChildren<Objective>().TitleText = entry.Value.Title;
                    temp.GetComponentInChildren<Objective>().Description = entry.Value.Description;
                    temp.GetComponentInChildren<Objective>().DetailSection = detailSectionText.GetComponent<Text>();
                    posY -= 80;
                }
            }
        }
        
        yield return null;
    }

    private void OnEnable()
    {
        StartCoroutine(LoadCurrentObjectives());
        if (GameController.Instance.currentGameOptions.inputSetting == InputType.Controller)
        {
            // select first objective
            if (objectivesCollection.Count > 0)
            {
                StartCoroutine(DelaySelectFirst(titleSection.GetComponentInChildren<Objective>().GetComponent<Button>()));
            }
        }
        else if (GameController.Instance.currentGameOptions.inputSetting == InputType.Keyboard)
        {
            
        }
    }

    IEnumerator DelaySelectFirst(Button btn)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(btn.gameObject);
    }
}
