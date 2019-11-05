using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NotificationType { None, NewObjective, UpdatedObjective, ObjectiveComplete, InventoryItemAdded, SkillPoints }

public class NewNotification
{

    public string message;
    public NotificationType notifType;

    public NewNotification()
    {
        message = "";
        notifType = NotificationType.None;
    }
    public NewNotification(string newMessage, NotificationType newNotifType)
    {
        message = newMessage;
        notifType = newNotifType;
    }
}

public class NotificationController : MonoBehaviour
{
    // public bool CreateNew;
    public GameObject notificationPrefab;
    public GameObject notificSection;

    public float posY = 450f; // this will need to be updated when a notification is set and removed
    public int numNotifications;
    const float MAX_NOTIFICATIONS = 6;
    float notificationDelay = 0f;
    float lastNotificationTime = 0f;
    float currentNotificationTime = 0f;

    List<NewNotification> notificationQueue = new List<NewNotification>();
    bool showingNotifications;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.CurrentGameState == GameState.Playing)
        {
            if (notificSection == null)
                notificSection = FindObjectOfType<UIController>().notificationPanel;

            if (notificationQueue.Count > 0 && !showingNotifications && notificSection != null)
            {
                showingNotifications = true;
                StartCoroutine(ShowNotifications(notificationQueue.Count));
            }

            numNotifications = NumberOfNotifications();

            if (!NotificationsExist())
            {
                ResetMaxPosY(true);
            }
        }
    }

    public void CreateNotification(string message, NotificationType notifType = NotificationType.None)
    {
        notificationQueue.Add(new NewNotification(message, notifType));
    }

    IEnumerator ShowNotifications(float count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject temp = Instantiate(notificationPrefab, notificSection.transform);
            temp.GetComponent<Notification>().MyNotificationText = GetMessagePrefix(notificationQueue[i].notifType) + notificationQueue[i].message;
            temp.GetComponent<Notification>().MaxYPos = posY;
            if (notificationQueue[i].notifType != NotificationType.None)
                temp.GetComponent<Notification>().ShowOptionsButton = true;
            else
                temp.GetComponent<Notification>().ShowOptionsButton = false;
            posY -= 70;
            yield return new WaitForSeconds(0.5f);
        }
        for(int i = 0; i < count; i++)
        {
            notificationQueue.RemoveAt(0);
        }
        showingNotifications = false;
    }

    IEnumerator AddToQueue(string message, NotificationType notifType)
    {
        if (Mathf.Abs(currentNotificationTime - lastNotificationTime) < 1f)
        {
            //create with delay
            float delay = (1 - Mathf.Abs(currentNotificationTime - lastNotificationTime)) + Mathf.Abs(currentNotificationTime - lastNotificationTime);
            yield return new WaitForSeconds(delay);
            lastNotificationTime = currentNotificationTime;
            GameObject temp = Instantiate(notificationPrefab, notificSection.transform);
            temp.GetComponent<Notification>().MyNotificationText = GetMessagePrefix(notifType) + message;
            temp.GetComponent<Notification>().MaxYPos = posY;
            if (notifType != NotificationType.None)
                temp.GetComponent<Notification>().ShowOptionsButton = true;
            else
                temp.GetComponent<Notification>().ShowOptionsButton = false;
            posY -= 70;
        }
        else
        {
            //no delay
            lastNotificationTime = currentNotificationTime;
            GameObject temp = Instantiate(notificationPrefab, notificSection.transform);
            temp.GetComponent<Notification>().MyNotificationText = GetMessagePrefix(notifType) + message;
            temp.GetComponent<Notification>().MaxYPos = posY;
            if (notifType != NotificationType.None)
                temp.GetComponent<Notification>().ShowOptionsButton = true;
            else
                temp.GetComponent<Notification>().ShowOptionsButton = false;
            posY -= 70;
            yield return null;
        }
    }

    string GetMessagePrefix(NotificationType notifType)
    {
        switch (notifType)
        {
            case NotificationType.NewObjective:
                return "New objective - ";
            case NotificationType.UpdatedObjective:
                return "Updated - ";
            case NotificationType.ObjectiveComplete:
                return "Completed - ";
            case NotificationType.InventoryItemAdded:
                return "New inventory item added - ";
            default:
                return "";
        }
    }


    bool NotificationsExist()
    {
        Notification[] allNotifications = FindObjectsOfType<Notification>();
        if (allNotifications.Length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    int NumberOfNotifications()
    {
        if (NotificationsExist())
            return FindObjectsOfType<Notification>().Length;
        else
            return 0;
    }

    bool NoNotificationAtThisPosition(float pos)
    {
        Notification[] allNotifications = FindObjectsOfType<Notification>();
        for (int i = 0; i < allNotifications.Length; i++)
        {
            if (allNotifications[i].MaxYPos == pos)
            {
                return false;
            }
        }
        return true;
    }

    public void ResetMaxPosY(bool resetComplete = false)
    {
        if (!resetComplete)
        {
            if (posY <= 450 - 70)
            {
                posY = FindFirstEmptyPos();
            }
        }
        else
        {
            posY = 450;
        }
    }

    float FindFirstEmptyPos()
    {
        for (int i = 0; i < MAX_NOTIFICATIONS; i++)
        {
            if (NoNotificationAtThisPosition(450 - (i * 70)))
            {
                return 450 - (i * 70);
            }
        }
        return posY + 70;
    }
}
