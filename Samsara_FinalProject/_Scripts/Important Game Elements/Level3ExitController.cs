using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3ExitController : MonoBehaviour
{

    public GameObject enemyBossPrefab;
    public GameObject enemyBossSpawnPoint;
    public GameObject teleporterToLock;

    public bool SpawnEnemiesOnce = false;
    public bool BeginBossBattle = false;
    public bool LevelComplete = false;

    GameObject myEnemyBoss;
    GateOpening gateOpening;
    SphereCollider entryPoint;

    int numEnemies = -1;

    // Use this for initialization
    void Start()
    {
        entryPoint = GetComponent<SphereCollider>();

        if (FindObjectOfType<BossController>() == null)
        {
            //myEnemyBoss = Instantiate(enemyBossPrefab, enemyBossSpawnPoint.transform.position, enemyBossSpawnPoint.transform.rotation);
        }
        else
        {
            myEnemyBoss = FindObjectOfType<BossController>().gameObject;
        }
        myEnemyBoss.SetActive(false);

        gateOpening = GameObject.FindGameObjectWithTag(GameObjectTag.GateOpening.ToString()).GetComponent<GateOpening>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!LevelComplete)
        {
            if (BeginBossBattle && myEnemyBoss.activeInHierarchy == false)
            {
                BeginBossBattle = true;
                StartCoroutine(StartBossBattle());
                teleporterToLock.SetActive(false);
            }

            //if (FindObjectOfType<BossController>().CurrentHealth <= 0)
            //{
            //if (!gateOpening.GateIsOpen)
            //gateOpening.GateIsOpen = true;
            //}
        }
        else if (LevelComplete)
        {
            if (GameObject.Find("Dialogue Point (2)") != null)
            {
                if (!GameObject.Find("Dialogue Point (2)").GetComponent<DialoguePoint>().inArea)
                {
                    // set the level exit end dialogue point to true so it plays
                    GameObject.Find("Dialogue Point (2)").GetComponent<DialoguePoint>().inArea = true;
                }
            }
        }
    }

    IEnumerator StartBossBattle()
    {
        yield return new WaitForSeconds(0.5f);
        myEnemyBoss.GetComponent<BossCutSceneAnimation>().BeginCutscene = true;
        myEnemyBoss.SetActive(true);
        myEnemyBoss.GetComponent<BossController>().enabled = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!BeginBossBattle)
            {
                BeginBossBattle = true;
            }
        }
    }

    public void OpenGate()
    {
        gateOpening.GateIsOpen = true;
    }
}
