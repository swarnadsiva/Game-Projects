using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUI : MonoBehaviour {

    public List<GameObject> panels;
    public List<GameObject> textButtons;

    //public bool PanelSelected { get; set; }

    private void Start()
    {
        //PanelSelected = false;
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetAxis("Horizontal") != 0)
        {

        }
    }

    IEnumerator MenuChange()
    {
      
        yield return null;
    }

    public void SetPanelSelected(string buttonName, string panelName)
    {
       // PanelSelected = true;
       
        foreach(GameObject textButton in textButtons)
        {
            if (textButton.name == buttonName)
            {
                textButton.GetComponent<PanelButtonController>().IsSelected = true;
            }
            else
            {
                textButton.GetComponent<PanelButtonController>().IsSelected = false;
            }
        }
    }
}
