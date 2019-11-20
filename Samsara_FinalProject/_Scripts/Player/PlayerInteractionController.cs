using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{

    public float moveSpeed = 2f;
    public bool NearObject;
    public bool HoldingObject;
    // public bool hasEnergyVesselHolder = false;
    public float maxInteractibleDistance;
    public GameObject heldObjectPosition;


    GameObject heldObject;
    PlayerCollisionControls collision;
    PlayerStatController playerStats;
    UIController uiControl;
    LayerMask interactiveMask;
    Animator anim;

    // Use this for initialization
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        collision = GetComponentInParent<PlayerCollisionControls>();
        playerStats = GetComponentInParent<PlayerStatController>();
        uiControl = GameObject.FindGameObjectWithTag(GameObjectTag.UIController.ToString()).GetComponent<UIController>();
        HoldingObject = false;
        NearObject = false;
        interactiveMask = LayerMask.GetMask(Layer.Movable.ToString()); // this is what we will use to ensure our raycasting only affects interactive objects
    }

    // Update is called once per frame
    void Update()
    {
        if (collision.CanInteract)
        {
            Vector3 localForward = transform.TransformVector(transform.worldToLocalMatrix.MultiplyVector(transform.forward));

            if (NearObject)
            {
                if (!HoldingObject && InputController.CheckCanPickupObject())
                {
                    RaycastHit[] outHits = GetObjectsNearPlayer(localForward);

                    CollectObjects(outHits);
                }
            }
            if (HoldingObject)
            {

                if (InputController.CheckDropObject())
                {
                    HoldingObject = false;
                    NearObject = true;
                    playerStats.CanPause = false;
                    StartCoroutine(DropObject());
                }
            }
        }
    }

    #region Picking up/Dropping objects

    public void PickupObject(Collider other)
    {
        anim.SetBool("IsPickup", true);
        //print(anim.GetBool("IsPickup"));
        StartCoroutine(ResetPickupAnimation());
        heldObject = other.gameObject;
        other.gameObject.transform.position = heldObjectPosition.transform.position;
        other.gameObject.transform.SetParent(transform);
        HoldingObject = true;
        NearObject = false;
        StartCoroutine(uiControl.ShowHUDInteractionText(true, "Drop", "drop"));
    }

    IEnumerator DropObject()
    {
        // release held object from being a child to the player object
        heldObject.transform.SetParent(null);

        // call functions to set rigidbody elements within the held object to make the object drop with gravity
        heldObject.GetComponent<MovableObjectController>().PlayerIsDroppingObject(transform.TransformVector(transform.worldToLocalMatrix.MultiplyVector(transform.forward)));

        // clear our local reference to the dropped object
        heldObject = null;

        //StartCoroutine(uiControl.ShowDialogueText(false));
        StartCoroutine(uiControl.ShowHUDInteractionText(false));
        yield return null;
    }

    IEnumerator ResetPickupAnimation(GameObject toSetInactive = null)
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("IsPickup", false);
        if (toSetInactive != null)
        {
            toSetInactive.SetActive(false);
        }
    }

    #endregion

    #region Collecting objects

    public RaycastHit[] GetObjectsNearPlayer(Vector3 localForward)
    {
        // get the object tho lift
        return Physics.SphereCastAll(transform.position, 2f, localForward, 2f, interactiveMask);
    }

    public void CollectObjects(RaycastHit[] outHits)
    {
        for (int i = 0; i < outHits.Length; i++)
        {

            if (outHits[i].collider.gameObject.tag == "Block")
            {
                PickupObject(outHits[i].collider);
                break;
            }
            else if (outHits[i].collider.gameObject.tag == "EnergyVessel")
            {
                CollectEnergyVessel(outHits[i].collider.gameObject);
                break;
            }
            else if (outHits[i].collider.gameObject.tag == "Energy")
            {
                if (InventoryController.Instance.PlayerHasEnergyVessel &&
                    outHits[i].collider.gameObject.GetComponent<EnergyOrbController>().CanPickUp)
                {

                    CollectEnergy(outHits[i].collider.gameObject);
                    break;
                }

            }
            else if (outHits[i].collider.gameObject.tag == "GoldenRock")
            {
                if (outHits[i].collider.gameObject.GetComponent<NewGoldenOrbController>().CanCollect)
                {
                    CollectGoldenOrb(outHits[i].collider.gameObject);
                    break;
                }
            }
            else if (outHits[i].collider.gameObject.tag == "PlayerRangedWeapon")
            {
                CollectRangedWeapon(outHits[i].collider.gameObject);
                break;
            }
        }
    }

    void CollectEnergyVessel(GameObject vessel)
    {
        if (!InventoryController.Instance.PlayerHasEnergyVessel)
        {
            // Inventory
            InventoryController.Instance.PlayerFoundItem(CollectibleItems.EnergyVessel);
            GameStatManager.Instance.PlayerSkillPoints += GameStatManager.EnergyVesselReward;
            FindObjectOfType<NotificationController>().CreateNotification("Skill points available");

            // Animation
            anim.SetBool("IsPickup", true);
            StartCoroutine(ResetPickupAnimation(vessel));

            // UI
            StartCoroutine(uiControl.ShowHUDInteractionText(false));
        }
    }

    void CollectEnergy(GameObject energy)
    {

        // Inventory
        if (energy.name.ToLower() == "energy1" && !InventoryController.Instance.PlayerHasFirstEnergy)
        {
            InventoryController.Instance.PlayerFoundItem(CollectibleItems.Energy1);
        }
        else if (energy.name.ToLower() == "energy2" && !InventoryController.Instance.PlayerHasSecondEnergy)
        {
            InventoryController.Instance.PlayerFoundItem(CollectibleItems.Energy2);
        }
        else if (energy.name.ToLower() == "energy3" && !InventoryController.Instance.PlayerHasThirdEnergy)
        {
            InventoryController.Instance.PlayerFoundItem(CollectibleItems.Energy3);
        }
        else
        {
            //print("player already has this item: " + energy.name);
            return;
        }

        GameStatManager.Instance.PlayerSkillPoints += GameStatManager.EnergyReward;
        FindObjectOfType<NotificationController>().CreateNotification("Skill points available");

        // for the orb statue puzzle at the end
        energy.transform.SetParent(transform);

        // Animation
        anim.SetBool("IsPickup", true);
        StartCoroutine(ResetPickupAnimation(energy));

        // UI
        StartCoroutine(uiControl.ShowHUDInteractionText(false));
    }

    void CollectGoldenOrb(GameObject orb)
    {
        if (!InventoryController.Instance.PlayerHasGoldenOrb)
        {
            // Inventory
            InventoryController.Instance.PlayerFoundItem(CollectibleItems.GoldenOrb);
            GameStatManager.Instance.PlayerSkillPoints += GameStatManager.GoldenOrbReward;
            FindObjectOfType<NotificationController>().CreateNotification("Skill points available");

            // Animation
            anim.SetBool("IsPickup", true);
            StartCoroutine(ResetPickupAnimation(orb));

            // UI
            StartCoroutine(uiControl.ShowHUDInteractionText(false));

        }
    }

    void CollectRangedWeapon(GameObject weapon)
    {
        if (!InventoryController.Instance.PlayerHasRangedWeapon)
        {
            InventoryController.Instance.PlayerFoundItem(CollectibleItems.RangedWeapon);

            //Animation
            anim.SetBool("IsPickup", true);
            StartCoroutine(ResetPickupAnimation(weapon));

            // UI
            StartCoroutine(uiControl.ShowHUDInteractionText(false));

            // ??? not sure why it's being parented
            //weapon.transform.SetParent(transform);

        }
    }

    #endregion




}
