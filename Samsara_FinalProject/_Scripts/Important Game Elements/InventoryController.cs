using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Item
{
    public Sprite IconImage { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public Item(Sprite spr, string itemName, string desc)
    {
        IconImage = spr;
        Name = itemName;
        Description = desc;
    }
}

public enum Inventory { Necklace, MeleeWeapon, EnergyVessel, Energy1, Energy2, Energy3, GoldenOrb, RangeWeapon }
public enum CollectibleItems { EnergyVessel, RangedWeapon, GoldenOrb, Energy1, Energy2, Energy3 };

public class InventoryController : MonoBehaviour
{

    public static InventoryController Instance;

    public Item[] allItems;
    public List<Item> currentInventory = new List<Item>();
    public List<Sprite> inventorySprites;

    public GameObject energyVessel;
    public GameObject rangedWeapon;
    public GameObject goldenOrb;
    public GameObject energyOrb1;
    public GameObject energyOrb2;
    public GameObject energyOrb3;

    public GameObject orb1Statue;
    public GameObject orb2Statue;
    public GameObject orb3Statue;

    Vector3 orb1StatuePosition;
    Vector3 orb2StatuePosition;
    Vector3 orb3StatuePosition;

    public bool PlayerHasRangedWeapon = false;
    public bool PlayerHasEnergyVessel = false;
    public bool PlayerHasGoldenOrb = false;
    public bool PlayerHasFirstEnergy = false;
    public bool PlayerHasSecondEnergy = false;
    public bool PlayerHasThirdEnergy = false;

    public bool Level1ExitComplete = false;

    public bool Orb1InStatue = false;
    public bool Orb2InStatue = false;
    public bool Orb3InStatue = false;

    public int EnergiesCollected;


    UIController uiControl;

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

        InitializeInventory(); // only do at beginning of game

        VerifyInventoryItemsInLevel(); // each time level is loaded (call from OnLevelLoaded in game controller

    }

    // Update is called once per frame
    void Update()
    {
        if (uiControl == null)
        {
            uiControl = FindObjectOfType<UIController>();
        }
    }

    public void ResetInventoryItems()
    {
        PlayerHasRangedWeapon = false;
        PlayerHasEnergyVessel = false;
        PlayerHasGoldenOrb = false;
        PlayerHasFirstEnergy = false;
        PlayerHasSecondEnergy = false;
        PlayerHasThirdEnergy = false;

        Level1ExitComplete = false;

        Orb1InStatue = false;
        Orb2InStatue = false;
        Orb3InStatue = false;
        EnergiesCollected = 0;

        currentInventory.Clear();

        InitializeInventory();
    }

    #region Initializing

    public void VerifyInventoryItemsInLevel()
    {
        if (SceneManager.GetActiveScene().name == ActiveGameScenes.Level1_New.ToString())
        {
            if (energyVessel == null)
                energyVessel = GameObject.FindGameObjectWithTag("EnergyVessel");
            if (rangedWeapon == null)
                rangedWeapon = GameObject.FindGameObjectWithTag("PlayerRangedWeapon");
            if (goldenOrb == null)
                goldenOrb = GameObject.FindGameObjectWithTag("GoldenRock");
            if (energyOrb1 == null)
                energyOrb1 = GameObject.Find("energy1");
            if (energyOrb2 == null)
                energyOrb2 = GameObject.Find("energy2");
            if (energyOrb3 == null)
                energyOrb3 = GameObject.Find("energy3");
            SetFoundObjectsInactive();
        }
    }

    void SetFoundObjectsInactive()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (PlayerHasEnergyVessel)
        {
            energyVessel.SetActive(false);
            GameObject.Find("Dialogue Point Collected Energy Vessel").SetActive(false);
            GameObject.Find("Dialogue Point Energy Vessel").SetActive(false);
            GameObject.Find("Dialogue Point after ranged weapon").SetActive(false);
        }
        if (PlayerHasRangedWeapon) // will need to deactivate guardian puzzle too
        {
            rangedWeapon.SetActive(false);
            FindObjectOfType<GuardianPuzzle>().EnsureGuardianPuzzleComplete();
            GameObject.Find("Dialogue Point Guardian").SetActive(false);
            GameObject.Find("Dialogue Point Guardian Puzzle Complete").SetActive(false);
        }

        if (PlayerHasGoldenOrb) // will need to deactive orb ambush too
        {
            goldenOrb.SetActive(false);
            GameObject.Find("Dialogue Point Golden Orb Ambush").SetActive(false);
            GameObject.Find("EnergyAmbush GoldenOrb").SetActive(false);
            GameObject.Find("Dialogue Point orb ambush complete").SetActive(false);
        }

        if (PlayerHasFirstEnergy && !Orb1InStatue)
        {
            energyOrb1.transform.SetParent(player.transform);
            energyOrb1.SetActive(false);
            GameObject.Find("Dialogue Point Without Vessel Near Energy").SetActive(false);
            GameObject.Find("Dialogue Point With Vessel Near Energy").SetActive(false);
            GameObject.Find("Dialogue Point First Enemy").SetActive(false);
        }
        else if (Orb1InStatue)
        {
            if (GameObject.Find("Dialogue Point Orb Near Statue") != null)
                GameObject.Find("Dialogue Point Orb Near Statue").SetActive(false);
            if (GameObject.Find("Dialogue Point Orb Statue") != null)
                GameObject.Find("Dialogue Point Orb Statue").SetActive(false);

            Collider[] temp = Physics.OverlapSphere(orb1StatuePosition, 1);
            if (temp.Length > 0)
            {
                foreach (Collider item in temp)
                {
                    if (item.gameObject.name.Contains("OrbStatue"))
                    {
                        orb1Statue = item.gameObject;
                        break;
                    }
                }
            }
            if (orb1Statue != null)
            {
                energyOrb1.SetActive(true);
                energyOrb1.transform.position = orb1StatuePosition;
                energyOrb1.transform.SetParent(orb1Statue.transform);
                energyOrb1.GetComponent<EnergyRandomMovement>().enabled = false;
                energyOrb1.GetComponent<EnergyOrbController>().InStatue = true;
                orb1Statue.GetComponent<OrbStatue>().HasOrb = true;
            }
            else
            {

                print("orb1statue is null");
            }

        }
        if (PlayerHasSecondEnergy && !Orb2InStatue)
        {
            energyOrb2.transform.SetParent(player.transform);
            energyOrb2.SetActive(false);
        }
        else if (Orb2InStatue)
        {
            if (GameObject.Find("Dialogue Point Orb Near Statue") != null)
                GameObject.Find("Dialogue Point Orb Near Statue").SetActive(false);
            if (GameObject.Find("Dialogue Point Orb Statue") != null)
                GameObject.Find("Dialogue Point Orb Statue").SetActive(false);
            Collider[] temp = Physics.OverlapSphere(orb2StatuePosition, 1);
            if (temp.Length > 0)
            {
                foreach (Collider item in temp)
                {
                    if (item.gameObject.name.Contains("OrbStatue"))
                    {
                        orb2Statue = item.gameObject;
                        break;
                    }
                }
            }
            if (orb2Statue != null)
            {
                energyOrb2.SetActive(true);
                energyOrb2.transform.position = orb2StatuePosition;
                energyOrb2.transform.SetParent(orb2Statue.transform);
                energyOrb2.GetComponent<EnergyRandomMovement>().enabled = false;
                energyOrb2.GetComponent<EnergyOrbController>().InStatue = true;
                orb2Statue.GetComponent<OrbStatue>().HasOrb = true;
            }
            else
            {
                print("orb2statue is null");
            }
        }
        if (PlayerHasThirdEnergy && !Orb3InStatue)
        {
            energyOrb3.transform.SetParent(player.transform);
            energyOrb3.SetActive(false);
        }
        else if (Orb3InStatue)
        {
            if (GameObject.Find("Dialogue Point Orb Near Statue") != null)
                GameObject.Find("Dialogue Point Orb Near Statue").SetActive(false);
            if (GameObject.Find("Dialogue Point Orb Statue") != null)
                GameObject.Find("Dialogue Point Orb Statue").SetActive(false);
            Collider[] temp = Physics.OverlapSphere(orb3StatuePosition, 1);
            if (temp.Length > 0)
            {
                foreach (Collider item in temp)
                {
                    if (item.gameObject.name.Contains("OrbStatue"))
                    {
                        orb3Statue = item.gameObject;
                        break;
                    }
                }
            }
            if (orb3Statue != null)
            {
                energyOrb3.SetActive(true);
                energyOrb3.transform.position = orb3StatuePosition;
                energyOrb3.transform.SetParent(orb3Statue.transform);
                energyOrb3.GetComponent<EnergyRandomMovement>().enabled = false;
                energyOrb3.GetComponent<EnergyOrbController>().InStatue = true;
                orb3Statue.GetComponent<OrbStatue>().HasOrb = true;
            }
            else
            {
                print("orb3statue is null");
            }
        }

        if (Level1ExitComplete)
        {
            GameObject.Find("Dialogue Point Gate Open").SetActive(false);
            StartCoroutine(FindObjectOfType<Level2ExitController>().OpenGate());
        }

        StartCoroutine(DelayedUpdatingEnergyIcons());
    }

    IEnumerator DelayedUpdatingEnergyIcons()
    {
        yield return new WaitForSeconds(0.5f);
        uiControl = FindObjectOfType<UIController>();
        uiControl.UpdateEnergyOrbsCollected(EnergiesCollected);
    }

    void SetOrbInStatue(GameObject energyOrb, GameObject statue, Vector3 statuePosition)
    {

    }

    // only once when game starts
    void InitializeInventory()
    {
        allItems = new Item[8];
        allItems[0] = new Item(inventorySprites[0], "Kala's Necklace", "This shining crystal was given to Kala by her mother on the eve of her 16th birthday. It appears to glow when looking closely at it.");
        allItems[1] = new Item(inventorySprites[1], "Mother's Sword", "Kala's mother carried this Macahuitl sword as a symbol of pride to the people of her village, but it fell into Kala's hands on the day her village was destroyed. It is flexible enough to use for a swift light attack, but sturdy enough to land powerful heavy blows.");
        allItems[2] = new Item(inventorySprites[2], "Spirit Vessel", "This holds the energy orbs of the lost and broken. Perhaps you can use it for something important in your journey.");
        allItems[3] = new Item(inventorySprites[3], "Energy Orb", "You stumbled across these orbs while traversing through the jungle ruins. The energy seems to carry the spirits of those from beyond the veil.");
        allItems[4] = new Item(inventorySprites[4], "Energy Orb", "You stumbled across these orbs while traversing through the jungle ruins. The energy seems to carry the spirits of those from beyond the veil.");
        allItems[5] = new Item(inventorySprites[5], "Energy Orb", "You stumbled across these orbs while traversing through the jungle ruins. The energy seems to carry the spirits of those from beyond the veil.");
        allItems[6] = new Item(inventorySprites[6], "Golden Orb", "These rocks are more than they appear to be. Intricate carvings indicate they were used for an ethereal purpose.");
        allItems[7] = new Item(inventorySprites[7], "Shakti Crossbow", "This was a gift from the Guardian of Strength. It resonates with power. To wield it, press L1 or E to lock onto a target, then R1 or R to fire at a nearby enemy.");

        // add to the current inventory (melee weapon, necklace)
        currentInventory.Add(allItems[(int)Inventory.Necklace]);
        currentInventory.Add(allItems[(int)Inventory.MeleeWeapon]);
    }

    #endregion

    #region Utilities

    public void PlayerFoundItem(CollectibleItems foundItem)
    {
        switch (foundItem)
        {
            case CollectibleItems.EnergyVessel:
                PlayerHasEnergyVessel = true;
                currentInventory.Add(allItems[(int)Inventory.EnergyVessel]);
                FindObjectOfType<NotificationController>().CreateNotification("Energy Vessel", NotificationType.InventoryItemAdded);
                break;
            case CollectibleItems.RangedWeapon:
                PlayerHasRangedWeapon = true;
                currentInventory.Add(allItems[(int)Inventory.RangeWeapon]);
                FindObjectOfType<NotificationController>().CreateNotification("Ranged Weapon", NotificationType.InventoryItemAdded);

                break;
            case CollectibleItems.GoldenOrb:
                PlayerHasGoldenOrb = true;
                currentInventory.Add(allItems[(int)Inventory.GoldenOrb]);
                FindObjectOfType<NotificationController>().CreateNotification("Golden Orb", NotificationType.InventoryItemAdded);

                break;
            case CollectibleItems.Energy1:
                PlayerHasFirstEnergy = true;
                EnergiesCollected++;
                currentInventory.Add(allItems[(int)Inventory.Energy1]);
                FindObjectOfType<NotificationController>().CreateNotification("Energy Orb", NotificationType.InventoryItemAdded);

                break;
            case CollectibleItems.Energy2:
                PlayerHasSecondEnergy = true;
                EnergiesCollected++;
                currentInventory.Add(allItems[(int)Inventory.Energy2]);
                FindObjectOfType<NotificationController>().CreateNotification("Energy Orb", NotificationType.InventoryItemAdded);

                break;
            case CollectibleItems.Energy3:
                PlayerHasThirdEnergy = true;
                EnergiesCollected++;
                currentInventory.Add(allItems[(int)Inventory.Energy3]);
                FindObjectOfType<NotificationController>().CreateNotification("Energy Orb", NotificationType.InventoryItemAdded);

                break;
            default:
                break;
        }
        
        uiControl = FindObjectOfType<UIController>();
        uiControl.UpdateEnergyOrbsCollected(EnergiesCollected);
    }

    public void RemoveEnergyOrbFromInventory(CollectibleItems orbToRemove, GameObject orbStatue)
    {
        EnergiesCollected--;
        switch (orbToRemove)
        {
            case CollectibleItems.Energy1:
                if (currentInventory.Contains(allItems[(int)Inventory.Energy1]))
                {
                    int index = currentInventory.IndexOf(allItems[(int)Inventory.Energy1]);
                    Orb1InStatue = true;
                    orb1Statue = orbStatue;
                    orb1StatuePosition = orb1Statue.GetComponent<OrbStatue>().orbPosition.transform.position;
                    PlayerHasFirstEnergy = false;
                    currentInventory.RemoveAt(index);
                }
                break;
            case CollectibleItems.Energy2:
                if (currentInventory.Contains(allItems[(int)Inventory.Energy2]))
                {
                    int index = currentInventory.IndexOf(allItems[(int)Inventory.Energy2]);
                    Orb2InStatue = true;
                    orb2Statue = orbStatue;
                    orb2StatuePosition = orb2Statue.GetComponent<OrbStatue>().orbPosition.transform.position;
                    PlayerHasSecondEnergy = false;
                    currentInventory.RemoveAt(index);
                }
                break;
            case CollectibleItems.Energy3:
                if (currentInventory.Contains(allItems[(int)Inventory.Energy3]))
                {
                    int index = currentInventory.IndexOf(allItems[(int)Inventory.Energy3]);
                    Orb3InStatue = true;
                    orb3Statue = orbStatue;
                    orb3StatuePosition = orb3Statue.GetComponent<OrbStatue>().orbPosition.transform.position;
                    PlayerHasThirdEnergy = false;
                    currentInventory.RemoveAt(index);
                }
                break;
            default:
                break;
        }
        uiControl.UpdateEnergyOrbsCollected(EnergiesCollected);
    }

    #endregion


}
