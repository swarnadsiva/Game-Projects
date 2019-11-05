using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianAreaMessage : MonoBehaviour {

     bool doneOnce = false;
    public bool InGuardianArea = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!doneOnce && other.gameObject.tag == "Player")
        {
            InGuardianArea = true;
            doneOnce = true;
            UIController uiControl = FindObjectOfType<UIController>();
            StartCoroutine(uiControl.ShowBeginningLevelMessage("Secret of the Guardian"));
        }

    }
}
