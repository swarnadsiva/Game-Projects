using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{

    protected NavMeshAgent agent;
    protected GameObject floor;
    protected float floorSize;
    protected bool useAIMovement = true; // default will need to be changed in inheriting classes

    Vector3 destination;
    float distToDest;
    public float minDistance = 15f;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        floor = GameObject.FindGameObjectWithTag("Floor");
        floorSize = (floor.transform.localScale.x * 10) / 2; // get one of the scale values (since this is going to be a square
        // and multiply by 10 since that's the grid value
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (useAIMovement)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                StartCoroutine(GotoNextPoint());
            }
        }
    }

    IEnumerator GotoNextPoint()
    {
        do
        {
            // find a random point on the floor
            float x = Random.Range(-floorSize, floorSize);
            float z = Random.Range(-floorSize, floorSize);

            // check that this point is more than 2f away
            destination = new Vector3(x, 0, z);

            distToDest = Vector3.Distance(transform.position, destination);

            Debug.LogFormat("({0}, 0, {1}) distance from current point: {2}", x, z, distToDest);

        } while (distToDest < minDistance);

        agent.destination = destination;

        yield return null;
    }
}
