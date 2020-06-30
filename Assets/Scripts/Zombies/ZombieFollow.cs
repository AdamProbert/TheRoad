using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class ZombieFollow : MonoBehaviour
{

    [SerializeField] Transform target;
    [SerializeField] float pathUpdateSpeed;

    NavMeshAgent agent;

    float timeToUpdate;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timeToUpdate = Time.time;
        NavMesh.avoidancePredictionTime = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > timeToUpdate)
        {
            timeToUpdate = Time.time + pathUpdateSpeed;
            agent.SetDestination(target.position);
        }   
    }
}
