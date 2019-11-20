using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// backup of ai pathing stuff



public class navAgentScript : MonoBehaviour {

    public float wanderRadius;
    public float wanderTimer;

    public GameObject player;
    public int moveSpeed = 2;

    private Transform target;
    private NavMeshAgent agent;
    private float timer;

    private Vector3 origPos;

    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            startWander();
        }

    }

    private void startWander()
    {
        Vector3 newPosition = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(newPosition);
        timer = 0;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * dist;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}
