using UnityEngine;
using UnityEngine.AI;

public class Patrolling : MonoBehaviour
{
    [SerializeField] private float detectionRange = 15f; // Full detection range
    [SerializeField] private Transform player; // Reference to the player
    private NavMeshAgent agent;
    private Vector3 spawnLocation; // Original spawn location
    private Vector3 patrolPoint;
    private bool isPlayerDetected = false;
    private float originalSpeed; // Store the agent's original speed
    private NavMeshQueryFilter queryFilter; // Query filter for NavMesh sampling

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed; // Save the original speed
        agent.speed = 5f; // Set patrolling speed
        spawnLocation = transform.position; // Store the original spawn location

        // Initialize NavMeshQueryFilter
        queryFilter = new NavMeshQueryFilter
        {
            areaMask = NavMesh.AllAreas,
            agentTypeID = agent.agentTypeID
        };

        SetRandomPatrolPoint();
    }

    void Update()
    {
        if (isPlayerDetected)
        {
            agent.speed = originalSpeed; // Revert to original speed
            agent.avoidancePriority = 50; // Disable wall avoidance
            agent.isStopped = true; // Stop patrolling
            return;
        }

        // Reduce detection range while patrolling
        float currentDetectionRange = detectionRange / 2;

        // Check if the player is within detection range
        if (Vector3.Distance(transform.position, player.position) <= currentDetectionRange)
        {
            isPlayerDetected = true;
            agent.avoidancePriority = 50; // Disable wall avoidance
            return;
        }

        // Move to the patrol point
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            agent.avoidancePriority = 0; // Enable wall avoidance
            SetRandomPatrolPoint(); // Set a new patrol point when the agent reaches the current one
        }
    }

    private void SetRandomPatrolPoint()
    {
        int maxAttempts = 10; // Limit the number of attempts
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomDirection = GetRandomPointOnNavMesh();

            if (Vector3.Distance(randomDirection, spawnLocation) >= 15f)
            {
                patrolPoint = randomDirection;
                agent.SetDestination(patrolPoint);
                return;
            }
        }
        // If no valid point found, fallback to spawn location
        patrolPoint = spawnLocation;
        agent.SetDestination(patrolPoint);
    }

    private Vector3 GetRandomPointOnNavMesh()
    {
        Vector3 randomDirection = transform.position + Random.insideUnitSphere * detectionRange;
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, detectionRange, queryFilter))
        {
            return hit.position;
        }

        return spawnLocation; // Fallback if no valid point is found
    }

    // Draw detection ranges in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange / 2); // Patrolling detection range

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Full detection range
    }
}