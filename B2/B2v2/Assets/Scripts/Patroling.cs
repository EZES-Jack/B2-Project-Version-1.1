using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AssignTargetAndReturn : MonoBehaviour
{
    [SerializeField] private float patrolRadius = 20f;
    public GameObject alertAreaObject;

    private NavMeshAgent agent;
    private Vector3 spawnLocation;
    private Vector3 targetLocation;
    private Vector3[] navMeshVertices;

    private static List<AssignTargetAndReturn> allAgents = new List<AssignTargetAndReturn>();
    private static bool alertActive = false;
    private static bool playerDetected = false; // New flag

    void Awake()
    {
        allAgents.Add(this);
    }

    void OnDestroy()
    {
        allAgents.Remove(this);
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("Missing NavMeshAgent. Disabling script.");
            enabled = false;
            return;
        }

        spawnLocation = transform.position;
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
        navMeshVertices = navMeshData.vertices;

        AssignRandomTarget();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (agent.destination == targetLocation)
            {
                agent.SetDestination(spawnLocation);
            }
            else
            {
                AssignRandomTarget();
            }
        }
    }

    private void AssignRandomTarget()
    {
        if (alertAreaObject != null && alertActive)
        {
            targetLocation = alertAreaObject.transform.position;
            agent.SetDestination(targetLocation);
            return;
        }

        // Stop patrolling if player has been detected
        if (playerDetected)
            return;

        int maxRetries = 10;
        for (int i = 0; i < maxRetries; i++)
        {
            int randomIndex = Random.Range(0, navMeshVertices.Length);
            Vector3 randomPoint = navMeshVertices[randomIndex];

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            {
                if (Vector3.Distance(spawnLocation, hit.position) >= 20f)
                {
                    targetLocation = hit.position;
                    agent.SetDestination(targetLocation);
                    return;
                }
            }
        }

        targetLocation = spawnLocation;
        agent.SetDestination(targetLocation);
    }

    public static void AlertAllAgents(GameObject areaObject)
    {
        alertActive = true;
        playerDetected = true; // Set flag on first detection
        foreach (var agent in allAgents)
        {
            agent.alertAreaObject = areaObject;
            agent.AssignRandomTarget();
            agent.enabled = false; // Disable this script
        }
    }

    public static void ClearAlert()
    {
        alertActive = false;
        foreach (var agent in allAgents)
        {
            agent.alertAreaObject = null;
            agent.AssignRandomTarget();
        }
    }
}