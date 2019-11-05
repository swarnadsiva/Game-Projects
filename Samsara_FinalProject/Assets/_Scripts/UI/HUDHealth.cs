using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDHealth : MonoBehaviour
{

    public List<Image> healthSpheres;

    public Sprite emptyHealthSphere;
    public Sprite fullHealthSphere;

    PlayerStatController playerStats;
    Color col = Color.red;
    int playerHealth;
    float timer = 0f;
    bool allHealthNormal = true;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerStats != null)
        {
            if (playerHealth != playerStats.currentHealth)
            {
                playerHealth = playerStats.currentHealth;
                if (playerHealth < 7) //not full
                {
                    for (int i = playerHealth - 1; i >= 0; i--)
                    {
                        healthSpheres[i].sprite = fullHealthSphere;
                    }

                    for (int i = 6; i >= playerHealth; i--)
                    {
                        healthSpheres[i].sprite = emptyHealthSphere;
                    }
                }
                else //all full
                {
                    foreach (Image item in healthSpheres)
                    {
                        item.sprite = fullHealthSphere;
                    }
                }
            }



            if (playerHealth <= 3)
            {
                timer += Time.deltaTime;
                allHealthNormal = false;
                //lerp between red and regular?
                for (int i = 0; i < healthSpheres.Count; i++)
                {
                    healthSpheres[i].color = Color.Lerp(healthSpheres[i].color, col, Time.deltaTime * 2f);
                }

                if (timer >= 1.5f)
                {
                    timer = 0f;
                    col = col == Color.red ? Color.white : Color.red;
                }

            }
            else if (!allHealthNormal)
            {
                
                for (int i = 0; i < healthSpheres.Count; i++)
                {
                    healthSpheres[i].color = Color.white;
                }
                allHealthNormal = true;
            }
        }
        else
        {
            playerStats = FindObjectOfType<PlayerStatController>();
        }
    }
}
