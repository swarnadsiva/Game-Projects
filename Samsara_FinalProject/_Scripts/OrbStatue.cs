using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbStatue : MonoBehaviour
{

    public Transform orbPosition;
    UIController uiControl;
    GameObject player;
    PlayerStatController playerStats;
    Level2ExitController exitCont;

    public bool HasOrb;
    public bool CanRotate;
    public float distance;
    const float MIN_DISTANCE = 5f;

    // Use this for initialization
    void Start()
    {
        foreach (Transform item in GetComponentsInChildren<Transform>())
        {
            if (item.gameObject.name.ToLower().Contains("position"))
            {
                orbPosition = item;
            }
        }
        exitCont = FindObjectOfType<Level2ExitController>();

        if (!HasOrb)
        {
            CanRotate = false;
        }
        else
        {
            CanRotate = true;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        if (playerStats == null)
            playerStats = player.GetComponent<PlayerStatController>();
        if (uiControl == null)
            uiControl = GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>();

        if (player != null)
        {
            distance = Vector3.Distance(player.transform.position, transform.position);
            if (InventoryController.Instance.EnergiesCollected > 0)
            {
                if (distance <= MIN_DISTANCE && !CanRotate)
                {

                    if (InputController.CheckCanPlaceOrb() && !HasOrb)
                    {
                        GameObject orb = GetOrbFromPlayer();
                        if (orb != null)
                        {
                            HasOrb = true;
                            StartCoroutine(exitCont.RemoveOrbFromPlayer(orbPosition, orb, this.gameObject));
                            StartCoroutine(uiControl.ShowHUDInteractionText(true, "Rotate Statue", "collect"));
                            StartCoroutine(DelayCanRotate());
                        }
                    }
                }
            }
        }
        if (CanRotate && !exitCont.PuzzleComplete)
        {
            distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance <= MIN_DISTANCE)
            {
                if (InputController.CheckCanRotateStatue())
                {
                    CanRotate = false;
                    StartCoroutine(RotateMe(Vector3.up * 45, 1f));
                }
            }
        }
        if (exitCont.PuzzleComplete && GameController.Instance.HudInteractionShowing)
        {
            StartCoroutine(uiControl.ShowHUDInteractionText(false));
        }

    }

    IEnumerator DelayCanRotate()
    {
        yield return new WaitForSeconds(0.2f);
        CanRotate = true;
    }

    IEnumerator RotateMe(Vector3 byAngles, float inTime)
    {
        var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);
        for (var t = 0f; t < 1; t += Time.deltaTime / inTime)
        {
            transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
            yield return null;
        }
        CanRotate = true;

    }

    public GameObject GetOrbFromPlayer()
    {
        EnergyOrbController[] temp = player.GetComponentsInChildren<EnergyOrbController>(true);
        if (temp.Length > 0)
        {
            temp[0].gameObject.SetActive(true);
            temp[0].transform.SetParent(this.transform);
            return temp[0].gameObject;
        }
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (InventoryController.Instance.EnergiesCollected > 0 && !CanRotate)
            {
                StartCoroutine(uiControl.ShowHUDInteractionText(true, "Place Orb", "collect"));
            }
            else if (CanRotate && !exitCont.PuzzleComplete)
            {
                StartCoroutine(uiControl.ShowHUDInteractionText(true, "Rotate Statue", "collect"));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && GameController.Instance.HudInteractionShowing)
        {
            StartCoroutine(uiControl.ShowHUDInteractionText(false));
        }
    }
}
