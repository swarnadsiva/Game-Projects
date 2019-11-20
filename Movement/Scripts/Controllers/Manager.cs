using UnityEngine;

public abstract class Manager : MonoBehaviour {

    public static Manager instance = null;

    // Use this for initialization
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // only one of each type of manager allowed
            Destroy(instance);
        }

        // persist this manager
        DontDestroyOnLoad(gameObject);
    }
}
