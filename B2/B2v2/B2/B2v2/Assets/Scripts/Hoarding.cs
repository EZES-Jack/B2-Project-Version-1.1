using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hoarding : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float groupingRadius = 5.0f;
    [SerializeField] private float maxSpeed = 5.0f;
    private List<NavMeshAgent> agents = new List<NavMeshAgent>();

    void Start()
    {
        NavMeshAgent[] foundAgents = FindObjectsOfType<NavMeshAgent>();
        agents.AddRange(foundAgents);
    }

    void Update()
    {
        if (target == null) return;

        if (agents.Count == 1)
        {
            agents[0].SetDestination(target.position);
            AdjustAgentSpeed(agents[0], target.position, 0f);
            return;
        }

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
            Vector3 direction = (agent.transform.position - centroid).normalized;
            Vector3 targetPosition = target.position + direction * Mathf.Min(groupingRadius, Vector3.Distance(agent.transform.position, target.position) / 2);
            agent.SetDestination(targetPosition);
            AdjustAgentSpeed(agent, target.position, maxDistance);
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
}