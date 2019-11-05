using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// wall will remove itself after the specified platforms have been activated
/// </summary>
public class WallController : MonoBehaviour
{

    public List<GameObject> rockPlatforms;

    private List<RockPlatformController> rockPlatformConts;

    private int frameCount = 0;
    public bool AllPlatformsActivated { get; set; }

    public bool NeedToCheckPlatforms { get; set; }

    // Use this for initialization
    void Start()
    {

    }


    private void Awake()
    {

        rockPlatformConts = new List<RockPlatformController>();
        foreach (GameObject rockPlatform in rockPlatforms)
        {
            rockPlatform.GetComponent<RockPlatformController>().LockedWall = this;
            rockPlatformConts.Add(rockPlatform.GetComponent<RockPlatformController>());
        }
        AllPlatformsActivated = false;
        NeedToCheckPlatforms = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (NeedToCheckPlatforms)
        {
            // only check every 3 seconds
            if (frameCount % 3 == 0)
            {
                AllPlatformsActivated = CheckPlatformsActivated(rockPlatformConts);
            }
            frameCount++;
        }

        if (AllPlatformsActivated)
        {
            NeedToCheckPlatforms = false;
            StartCoroutine(WallSinkDown());
            Destroy(gameObject, 5f);
        }
    }

    IEnumerator WallSinkDown()
    {
        Vector3 destination = new Vector3(transform.position.x, transform.position.y - 10f, transform.position.z);
        Vector3 currentPosition = transform.position;
        transform.position = Vector3.Lerp(currentPosition, destination, Time.deltaTime * 0.5f);
        yield return null;
        // yield return new WaitForSeconds(5f);
        //gameObject.SetActive(false); // instead of destroying object, set it inactive
    }

    bool CheckPlatformsActivated(List<RockPlatformController> rockPlatforms)
    {
        foreach (RockPlatformController item in rockPlatforms)
        {
            if (!item.HasInteractiveRock)
            {
                return false;
            }
        }
        return true;
    }
}
