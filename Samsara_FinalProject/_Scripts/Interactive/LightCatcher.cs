using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCatcher : MonoBehaviour {
    Material originalMat;
    Material lightCastMat;
    Level2ExitController exitCont;
    Renderer rend;
    Light lightComp;
    public bool IsHit;
    public int HitNumber = 0;
    bool doOnce = false;

	// Use this for initialization
	void Start () {

        lightComp = GetComponent<Light>();
        exitCont = FindObjectOfType<Level2ExitController>();
    }
	
	// Update is called once per frame
	void Update () {
		if (HitNumber > 0)
        {
         
            if (lightComp.range <= HitNumber * 1.5f * 2.5f)
            {
                lightComp.range += 0.1f;
            }
            if (lightComp.intensity > HitNumber * 1.5f * 0.65f)
            {
                lightComp.intensity += 0.1f;
            }
        }
        else
        {
            if (lightComp.range > 2)
            {
                lightComp.range -= 0.1f;
            }
            if (lightComp.intensity > 0.5)
            {
                lightComp.intensity -= 0.1f;
            }
        }

        if (HitNumber >= 3 && !doOnce)
        {
            doOnce = true;
            exitCont.StartCoroutine(exitCont.OpenGate());
        }

	}
}
