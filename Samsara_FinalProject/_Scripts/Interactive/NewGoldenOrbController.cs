using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGoldenOrbController : MonoBehaviour
{

    public bool CanCollect = false;
    public bool AmbushStarted = false;
    public GameObject goldenOrbAmbush;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 50 * Time.deltaTime, 0);
       

        if (AmbushStarted)
        {
            //check if all enemies are dead
            if (!CanCollect)
            {
                if (CheckAllEnemiesDead())
                    CanCollect = true;
            }
        }
        else
        {
            AmbushStarted = CheckAmbushStarted();
        }
    }

    public bool CheckAmbushStarted()
    {
        List<GameObject> ambushEnemies = goldenOrbAmbush.GetComponent<EnemyAmbushPoint>().enemiesToActivate;
        foreach (GameObject item in ambushEnemies)
        {
            if (item.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckAllEnemiesDead()
    {
        List<GameObject> ambushEnemies = goldenOrbAmbush.GetComponent<EnemyAmbushPoint>().enemiesToActivate;
        foreach (GameObject item in ambushEnemies)
        {
            if (item.activeInHierarchy)
            {
                return false;
            }
        }
        return true;
    }
}
