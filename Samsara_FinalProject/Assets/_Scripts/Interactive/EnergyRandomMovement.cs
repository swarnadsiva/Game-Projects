using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyRandomMovement : MonoBehaviour
{

    float yPosInitial;
    float xPosInitial;
    float zPosInitial;

    float speed = 0.5f;
    float amplitude = 1.5f;

    public float XPValue = 50f;
    private bool m_movetoend;
    public bool MoveToEndPosition
    {
        get
        {
            return m_movetoend;
        }
        set
        {
            m_movetoend = value;
            timer = 0;
        }
    }
    public bool AtEndPosition;
    public bool Collected;

    public Vector3 EnergyLevelEndPosition;

    GameObject player;
    PlayerStatController playerStats;
    Vector3 beginningPosition;

    float timer = 0f;
    float timerBegPos = 0f;
   public bool restartRandomMovement = true;
    public bool goToBeginningPos = false;

    private const float MIN_DISTANCE = 8f;

    void Start()
    {
        beginningPosition = transform.position;
        Collected = false;
        MoveToEndPosition = false;
        AtEndPosition = false;
        StartCoroutine(LoadElements());
        speed = Random.Range(0.2f, 0.8f);
    }

    IEnumerator LoadElements()
    {
        yield return new WaitForSeconds(0.2f);
        yPosInitial = transform.position.y;
        xPosInitial = transform.position.x;
        zPosInitial = transform.position.z;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        playerStats = player.GetComponent<PlayerStatController>();
    }

    private void LateUpdate()
    {
        if (player != null && !Collected)
        {
            if (InventoryController.Instance.PlayerHasEnergyVessel)
            {
                if (Vector3.Distance(transform.position, player.transform.position) < MIN_DISTANCE && !goToBeginningPos)
                {
                    // float towards the player
                    transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x + 2f, player.transform.position.y + 2f, player.transform.position.z), Time.deltaTime * 1.5f);
                    restartRandomMovement = false;
                    if (Vector3.Distance(transform.position, beginningPosition) > MIN_DISTANCE)
                    {
                        goToBeginningPos = true;
                    }
                }
                else
                {
                    if (!restartRandomMovement && goToBeginningPos)
                    {
                        timerBegPos += Time.deltaTime;
                        // go back to original spot
                        transform.position = Vector3.Lerp(transform.position, beginningPosition, Time.deltaTime * 2f);
                        if (timerBegPos >= 1.5f)
                        {
                            timerBegPos = 0f;
                            timer = 0f;
                            restartRandomMovement = true;
                            goToBeginningPos = false;
                        }
                    }
                }

                if (restartRandomMovement)
                {
                    DoRandomMovement();
                }
            }
            else
            {
                DoRandomMovement();
            }
        }
    }

    private void DoRandomMovement()
    {
        timer += Time.deltaTime;
        // random movement
        float newX = xPosInitial + amplitude * Mathf.Sin(speed * timer * 2);
        float newY = yPosInitial + amplitude * Mathf.Sin(speed * timer);
        float newZ = zPosInitial + amplitude * Mathf.Sin(speed * timer * 3);
        transform.position = new Vector3(newX, newY, newZ);
    }

    private void OnDisable()
    {
        Collected = true;
    }
}
