using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public enum ObjectiveTitle { JungleRuins, MysteriousEnergies, TheGuardianSecret, RestoringBalance };

public class ObjectiveController : MonoBehaviour
{

    public static ObjectiveController Instance;
    public GameObjective[] allObjectives = new GameObjective[4];
    public Dictionary<ObjectiveTitle, GameObjective> currentObjectives = new Dictionary<ObjectiveTitle, GameObjective>();

    bool trigStartJungle = false;
    bool trigStartMystEnerg = false;
    bool trigStartGuardSecret = false;
    bool trigStartRestoreBalance = false;

    bool trigUpdateMystEnerg1 = false;
    bool trigUpdateMystEnerg2 = false;
    bool trigUpdateGuardSecr1 = false;
    bool trigUpdateRestorreBalance1 = false;

    bool trigCompMystEnerg = false;
    bool trigCompGuardSecret = false;
    bool trigCompRestBalance = false;


    GameObject player;
    GameObject guardian;
    GameObject level1Exit;
    PlayerStatController playerStats;
    GuardianAreaMessage guardMessage;
    GuardianPuzzle guardPuzzle;

    BossController boss;

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

        allObjectives[0] = new GameObjective("Jungle Ruins", "Kala's tribe was annihilated by the Arati, a force of great destruction that has ruined her land. \n\n The only survivors were her and her mother, but Kala's mother sacrificed herself to give her a chance to escape. \n\n With no land to call her home, Kala must wander the wildnerness in search of answers for what happened to her family.");
        allObjectives[1] = new GameObjective("Mysterious Energies", "When Kala was a child, her mother told her stories about an ancient vessel that possessed the power to restore balance to the land. \n\nThis vessel collect remnants of energy called 'Energy Orbs.' It is said that the Orbs contain the very essence of the jungle itself.", "\nFind the remaining energy orbs.", "\nThe energies you have gathered seem to be pulling you in a certain direction. Perhaps they are leading you somewhere. ");
        allObjectives[2] = new GameObjective("The Guardian's Secret", "Kala stumbled upon an ancient stone statue that feels eerily alive. \nPerhaps there is more to this statue than meets the eye.", "\nYou've discovered that the statue responds when you step on the marked tiles near it.");
        allObjectives[3] = new GameObjective("Restoring Balance", "The air in this place feels suffocating. The necklace Kala's mother gave her seems to flash uneasily in this area, a sign of chaotic energy. \nExplore this area to find the cause of the restlessness.", "\nA minion of the Arati force has appeared! It seeks to finish the destruction that was started in Kala's village. Defeat him before he kills you!");

        guardMessage = FindObjectOfType<GuardianAreaMessage>();
        guardPuzzle = FindObjectOfType<GuardianPuzzle>();
        boss = FindObjectOfType<BossController>();
    }

    private void LateUpdate()
    {
        if (player == null && GameController.Instance.CurrentGameState == GameState.Playing)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerStats = player.GetComponent<PlayerStatController>();
        }
    }

    private void Update()
    {
        // need to check for the other trigger points
        if (guardMessage == null)
        {
            guardMessage = FindObjectOfType<GuardianAreaMessage>();
        }
        if (guardPuzzle == null)
        {
            guardPuzzle = FindObjectOfType<GuardianPuzzle>();
        }
        if (boss == null)
        {
            boss = FindObjectOfType<BossController>();
        }
        if (player != null)
        {
            if (SceneManager.GetActiveScene().name == ActiveGameScenes.Level1_New.ToString())
            {
                if (!trigStartJungle)
                {
                    trigStartJungle = true;
                    StartObjective(ObjectiveTitle.JungleRuins);
                }

                // Mysterious Energies --> after collecting the energy vessel
                // Mysterious Energies update 1 --> after collecting one energy orbs
                // Mysterious Energies update 2 --> after collecting all energy orbs
                // Mysterious Energies complete --> after player completes level 1 exit
                if (InventoryController.Instance.PlayerHasEnergyVessel && !trigStartMystEnerg)
                {
                    trigStartMystEnerg = true;
                    StartObjective(ObjectiveTitle.MysteriousEnergies);
                    FindObjectOfType<NotificationController>().CreateNotification("Mysterious Energies", NotificationType.NewObjective);
                }
                else if (InventoryController.Instance.PlayerHasEnergyVessel && InventoryController.Instance.PlayerHasFirstEnergy && !trigUpdateMystEnerg1)
                {
                    trigUpdateMystEnerg1 = true;
                    UpdateObjective(ObjectiveTitle.MysteriousEnergies, 1);
                    FindObjectOfType<NotificationController>().CreateNotification("Mysterious Energies", NotificationType.UpdatedObjective);
                }
                else if (InventoryController.Instance.PlayerHasEnergyVessel && InventoryController.Instance.PlayerHasFirstEnergy &&
                    InventoryController.Instance.PlayerHasSecondEnergy &&
                    InventoryController.Instance.PlayerHasThirdEnergy && !trigUpdateMystEnerg2)
                {
                    trigUpdateMystEnerg2 = true;
                    UpdateObjective(ObjectiveTitle.MysteriousEnergies, 2);
                    FindObjectOfType<NotificationController>().CreateNotification("Mysterious Energies", NotificationType.UpdatedObjective);
                }
                else if (InventoryController.Instance.PlayerHasEnergyVessel && InventoryController.Instance.PlayerHasFirstEnergy &&
                    InventoryController.Instance.PlayerHasSecondEnergy &&
                    InventoryController.Instance.PlayerHasThirdEnergy && !trigCompGuardSecret)
                {
                    trigCompGuardSecret = true;
                    CompleteObjective(ObjectiveTitle.MysteriousEnergies);
                    FindObjectOfType<NotificationController>().CreateNotification("Mysterious Energies", NotificationType.ObjectiveComplete);
                }

                // Guardian's Secret --> after finding guardian statue
                // Guardian's Secret update 1 --> after stepping on first tile
                // Guardian's Secret complete --> after completing guardian puzzle & dialogue
                if (guardMessage.InGuardianArea && !trigStartGuardSecret)
                {
                    trigStartGuardSecret = true;
                    StartObjective(ObjectiveTitle.TheGuardianSecret);
                    FindObjectOfType<NotificationController>().CreateNotification("The Guardian's Secret", NotificationType.NewObjective);
                }
                else if (guardPuzzle.OrderStarted && !trigUpdateGuardSecr1)
                {
                    trigUpdateGuardSecr1 = true;
                    UpdateObjective(ObjectiveTitle.TheGuardianSecret, 1);
                    FindObjectOfType<NotificationController>().CreateNotification("The Guardian's Secret", NotificationType.UpdatedObjective);

                }
                else if (guardPuzzle.PuzzleComplete && !trigCompGuardSecret)
                {
                    trigCompGuardSecret = true;
                    CompleteObjective(ObjectiveTitle.TheGuardianSecret);
                    FindObjectOfType<NotificationController>().CreateNotification("The Guardian's Secret", NotificationType.ObjectiveComplete);

                }
            }


            // Restoring Balance --> after entering level 3
            // Restoring Balance update ---> after boss cutscene
            // Restoring Balance complete --> defeating boss
            if (SceneManager.GetActiveScene().name == ActiveGameScenes.Level3.ToString() && !trigStartRestoreBalance)
            {
                trigStartRestoreBalance = true;
                StartObjective(ObjectiveTitle.RestoringBalance);
                FindObjectOfType<NotificationController>().CreateNotification("Restoring Balance", NotificationType.NewObjective);
            }
            else if (SceneManager.GetActiveScene().name == ActiveGameScenes.Level3.ToString() && boss != null && boss.gameObject.activeInHierarchy && !trigUpdateRestorreBalance1)
            {
                trigUpdateRestorreBalance1 = true;
                UpdateObjective(ObjectiveTitle.RestoringBalance, 1);
                FindObjectOfType<NotificationController>().CreateNotification("Restoring Balance", NotificationType.UpdatedObjective);
            }
            else if (SceneManager.GetActiveScene().name == ActiveGameScenes.Level3.ToString() && boss != null && boss.CurrentHealth <= 0 && !trigCompRestBalance)
            {
                trigCompRestBalance = true;
                CompleteObjective(ObjectiveTitle.RestoringBalance);
                FindObjectOfType<NotificationController>().CreateNotification("Restoring Balance", NotificationType.ObjectiveComplete);
            }
        }

    }


    public void StartObjective(ObjectiveTitle objTitle)
    {
        currentObjectives.Add(objTitle, allObjectives[(int)objTitle]);
        int index = currentObjectives.Count - 1;
        currentObjectives[objTitle].MakeActive();
    }

    public void UpdateObjective(ObjectiveTitle objTitle, int updateNum)
    {
        currentObjectives[objTitle].UpdateDescription(updateNum);
    }

    public void CompleteObjective(ObjectiveTitle objTitle)
    {
        currentObjectives[objTitle].MakeComplete();
    }

    public void ResetObjectives()
    {
        trigStartJungle = false;
        trigStartMystEnerg = false;
        trigStartGuardSecret = false;
        trigStartRestoreBalance = false;

        trigUpdateMystEnerg1 = false;
        trigUpdateMystEnerg2 = false;
        trigUpdateGuardSecr1 = false;
        trigUpdateRestorreBalance1 = false;

        trigCompMystEnerg = false;
        trigCompGuardSecret = false;
        trigCompRestBalance = false;

        currentObjectives.Clear();
    }
}

public class GameObjective
{
    public string Title;
    public string Description;

    public string InitialDescription;
    public string DescAddition1;
    public string DescAddition2;
    public bool Active;

    public GameObjective()
    {
        //empty constructor
    }

    public GameObjective(string myTitle, string myInitDesc, string myDescAdd1 = "", string myDescAdd2 = "")
    {
        Title = myTitle;
        InitialDescription = myInitDesc;
        DescAddition1 = myDescAdd1;
        DescAddition2 = myDescAdd2;
        Active = false;
    }

    public void MakeActive()
    {
        Description = InitialDescription;
        Active = true;
    }

    public void UpdateDescription(int updateNumber)
    {
        if (updateNumber == 1)
        {
            Description += "\n" + DescAddition1;
        }
        else if (updateNumber == 2)
        {
            Description += "\n" + DescAddition2;
        }
    }

    public void MakeComplete()
    {
        Active = false;
        Description += "\nObjective completed.";
    }
}
