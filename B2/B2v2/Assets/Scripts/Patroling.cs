using UnityEngine;
using UnityEngine.AI;

public class AssignTargetAndReturn : MonoBehaviour
{
    [SerializeField] private float patrolRadius = 20f; // Radius for random target locations
    private NavMeshAgent agent;
    private Vector3 spawnLocation;
    private Vector3 targetLocation;
    private Vector3[] navMeshVertices;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("Missing NavMeshAgent. Disabling script.");
            enabled = false;
            return;
        }

        spawnLocation = transform.position; // Save the spawn location

        // Cache NavMesh vertices for performance
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
        navMeshVertices = navMeshData.vertices;

        AssignRandomTarget(); // Assign the first random target
    }

    void Update()
    {
        if (CompareTag("Variant"))
        {
            Ai2 ai2 = GetComponent<Ai2>();
            if (ai2 != null && ai2.playerDetected)
            {
                DisableAgent();
                return;
            }
        }
        else if (CompareTag("Enemy"))
        {
            ZombieAI zombieAI = GetComponent<ZombieAI>();
            if (zombieAI != null && zombieAI.playerDetected)
            {
                DisableAgent();
                return;
            }
        }

        // Check if the agent has reached its destination
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (agent.destination == targetLocation)
            {
                agent.SetDestination(spawnLocation); // Return to spawn location
            }
            else
            {
                AssignRandomTarget(); // Assign a new random target
            }
        }
    }

    private void AssignRandomTarget()
    {
        int maxRetries = 10; // Limit the number of retries

        for (int i = 0; i < maxRetries; i++)
        {
            // Generate a random point from cached NavMesh vertices
            int randomIndex = Random.Range(0, navMeshVertices.Length);
            Vector3 randomPoint = navMeshVertices[randomIndex];

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            {
                if (Vector3.Distance(spawnLocation, hit.position) >= 20f) // Ensure the target is at least 20f away
                {
                    targetLocation = hit.position;
                    agent.SetDestination(targetLocation);
                    return;
                }
            }
        }

        // Fallback: If no valid target is found, return to spawn location
        Debug.LogWarning("Failed to find a valid target location after max retries. Returning to spawn.");
        targetLocation = spawnLocation;
        agent.SetDestination(targetLocation);
    }

    private void DisableAgent()
    {
        agent.isStopped = true;
        enabled = false;
    }
}