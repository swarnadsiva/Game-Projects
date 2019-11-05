using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameStatManager : MonoBehaviour
{

    public static GameStatManager Instance;

    public int PlayerSkillPoints;
    public int AttackSpeed;
    public int AttackDamage;
    public int MovementSpeed;

    public bool IsPlayerRespawn;

    public const int EnergyVesselReward = 1;
    public const int EnergyReward = 1;
    public const int GoldenOrbReward = 2;
    public const int OrbStatuePuzzle = 1;

    // Use this for initialization
    void Awake()
    {

        // ensures there is only one instance of our GameObject at any given time.
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        ResetStats();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance != null)
        {
            if (GameController.Instance.CurrentGameState == GameState.GameOver)
            {
                IsPlayerRespawn = true;
            }
        }
    }

    public void ResetStats()
    {
        PlayerSkillPoints = 0;
        AttackSpeed = 0;
        AttackDamage = 0;
        MovementSpeed = 0;
        IsPlayerRespawn = false;
    }
}
