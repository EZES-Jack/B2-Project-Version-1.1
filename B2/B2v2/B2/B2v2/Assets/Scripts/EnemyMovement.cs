using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float groupingRadius = 5.0f;
    [SerializeField] private float maxSpeed = 5.0f;
    private List<NavMeshAgent> agents = new List<NavMeshAgent>();
    private CommunicationManager communicationManager;
    private Dictionary<NavMeshAgent, Vector3> initialRelativePositions = new Dictionary<NavMeshAgent, Vector3>();
    public GameObject player;
    [SerializeField] private float stoppingDistance = 2.0f;

    void Start()
    {
        NavMeshAgent[] foundAgents = FindObjectsOfType<NavMeshAgent>();
        agents.AddRange(foundAgents);
        Debug.Log($"Found {agents.Count} agents.");

        communicationManager = FindObjectOfType<CommunicationManager>();
        if (communicationManager == null)
        {
            Debug.LogError("CommunicationManager not found!");
            return;
        }

        foreach (NavMeshAgent agent in agents)
        {
            communicationManager.RegisterAgent(agent);
        }

        StoreInitialRelativePositions();
    }

    void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("Target is not assigned.");
            return;
        }

        MaintainFormation();
        HandleEnemyAgents();
    }

    private void StoreInitialRelativePositions()
    {
        Vector3 centroid = CalculateCentroid();
        foreach (NavMeshAgent agent in agents)
        {
            initialRelativePositions[agent] = agent.transform.position - centroid;
        }
    }

    private void MaintainFormation()
    {
        Vector3 centroid = CalculateCentroid();
        float maxDistance = 0f;

        foreach (NavMeshAgent agent in agents)
        {
            float distance = Vector3.Distance(agent.transform.position, target.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
            }
        }

        foreach (NavMeshAgent agent in agents)
        {
            float distanceToTarget = Vector3.Distance(agent.transform.position, target.position);
            if (distanceToTarget <= groupingRadius)
            {
                agent.SetDestination(target.position);
            }
            else
            {
                Vector3 targetPosition = target.position + initialRelativePositions[agent] * 0.5f;
                Vector3 adjustedPosition = communicationManager.GetAdjustedDestination(agent, targetPosition);
                agent.SetDestination(adjustedPosition);
                communicationManager.UpdateAgentDestination(agent, adjustedPosition);
            }
            AdjustAgentSpeed(agent, target.position, maxDistance);
        }
    }

    private void HandleEnemyAgents()
    {
        foreach (NavMeshAgent agent in agents)
        {
            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(agent.transform.position, player.transform.position);
                if (distanceToPlayer > stoppingDistance)
                {
                    Ray ray = new Ray(agent.transform.position, player.transform.position - agent.transform.position);
                    if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, LayerMask.GetMask("Player")))
                    {
                        agent.SetDestination(hit.point);
                        agent.isStopped = false;
                    }
                }
                else
                {
                    agent.isStopped = true;
                }
            }
        }
    }

    private Vector3 CalculateCentroid()
    {
        Vector3 centroid = Vector3.zero;
        foreach (NavMeshAgent agent in agents)
        {
            centroid += agent.transform.position;
        }
        return centroid / agents.Count;
    }

    private void AdjustAgentSpeed(NavMeshAgent agent, Vector3 targetPosition, float maxDistance)
    {
        if (maxDistance > 0)
        {
            float distanceToTarget = Vector3.Distance(agent.transform.position, targetPosition);
            agent.speed = maxSpeed * (distanceToTarget / maxDistance);
        }
        else
        {
            agent.speed = maxSpeed;
        }
    }

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.green;
            DrawWireCircle(target.position, groupingRadius);

            Gizmos.color = Color.red;
            foreach (NavMeshAgent agent in agents)
            {
                Gizmos.DrawLine(agent.transform.position, target.position);
            }
        }
    }

    private void DrawWireCircle(Vector3 position, float radius)
    {
        int segments = 36;
        float angle = 0f;
        Vector3 lastPoint = position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
        for (int i = 1; i <= segments; i++)
        {
            angle = i * Mathf.PI * 2 / segments;
            Vector3 nextPoint = position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (NavMeshAgent agent in agents)
            {
                agent.isStopped = true;
            }
        }
    }
}