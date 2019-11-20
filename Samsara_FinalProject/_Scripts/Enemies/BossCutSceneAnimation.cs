using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossCutSceneAnimation : MonoBehaviour
{

    NavMeshAgent agent;
    Animator anim;
    bool doOnce;
    public bool BeginCutscene = false;

     GameObject destination1;

    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        destination1 = GameObject.Find("walk to position 2");
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance == 0)
        {
            if (anim.GetBool("IsWalking"))
            {
                anim.SetBool("IsWalking", false);
            }
        }
    }

    private void OnDisable()
    {
        if (!anim.GetBool("IsWalking"))
        {
            anim.SetBool("IsWalking", true);
        }
    }

    private void OnEnable()
    {
        if (BeginCutscene)
        {
            if (!doOnce)
            {
                doOnce = true;
                StartCoroutine(BeginBossCutscene());
            }
        }
    }

    IEnumerator BeginBossCutscene()
    {
        anim.SetBool("IsWalking", true);
        agent.SetDestination(destination1.transform.position);
        yield return new WaitForSeconds(16f);
        anim.SetTrigger("Roar");
        yield return new WaitForSeconds(5f);
        print("disabling boss cutscene script");
        GetComponent<BossController>().enabled = true;
        this.enabled = false;
    }
}
