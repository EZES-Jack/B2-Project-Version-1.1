using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAgent : MonoBehaviour
{
    public GameObject player;
    private NavMeshAgent agent;
    [SerializeField] private float stoppingDistance = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(agent.transform.position, player.transform.position);
            if (distanceToPlayer > stoppingDistance)
            {
                Ray ray = new Ray(transform.position, player.transform.position - transform.position);
                Physics.Raycast(ray, out RaycastHit hit, Single.PositiveInfinity, LayerMask.GetMask("Player"));
                agent.SetDestination(hit.point);
                agent.isStopped = false;
            }
            else
            {
                agent.isStopped = true;
            }
        } 
    }
}
