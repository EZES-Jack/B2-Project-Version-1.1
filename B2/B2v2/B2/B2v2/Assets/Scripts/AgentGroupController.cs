using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentGroupController : MonoBehaviour
{
    // Target that agents will follow
    public Transform target;

    // Radius within which agents will group
    [SerializeField] private float groupingRadius = 2.0f; // Reduced radius for closer grouping

    // Maximum speed of the agents
    [SerializeField] private float maxSpeed = 5.0f;

    // List to store all agents
    private List<NavMeshAgent> agents = new List<NavMeshAgent>();

    // Reference to the CommunicationManager
    private CommunicationManager communicationManager;

    // Dictionary to store initial relative positions of agents
    private Dictionary<NavMeshAgent, Vector3> initialRelativePositions = new Dictionary<NavMeshAgent, Vector3>();

    void Start()
    {
        // Find all NavMeshAgents in the scene and add them to the list
        NavMeshAgent[] foundAgents = FindObjectsOfType<NavMeshAgent>();
        agents.AddRange(foundAgents);

        // Find the CommunicationManager in the scene
        communicationManager = FindObjectOfType<CommunicationManager>();

        // Register each agent with the CommunicationManager
        foreach (NavMeshAgent agent in agents)
        {
            communicationManager.RegisterAgent(agent);
        }

        // Store initial relative positions of agents
        StoreInitialRelativePositions();
    }

    void Update()
    {
        // If there is no target, do nothing
        if (target == null) return;

        // Maintain formation
        MaintainFormation();
    }

    // Store the initial relative positions of agents
    private void StoreInitialRelativePositions()
    {
        Vector3 centroid = CalculateCentroid();
        foreach (NavMeshAgent agent in agents)
        {
            initialRelativePositions[agent] = agent.transform.position - centroid;
        }
    }

    // Maintain the formation of agents
    private void MaintainFormation()
    {
        Vector3 centroid = CalculateCentroid();
        foreach (NavMeshAgent agent in agents)
        {
            float distanceToTarget = Vector3.Distance(agent.transform.position, target.position);
            if (distanceToTarget <= groupingRadius)
            {
                // If within grouping radius, move directly to the player's position
                agent.SetDestination(target.position);
                
            }
            else
            {
                // Otherwise, maintain initial relative position
                Vector3 targetPosition = target.position + initialRelativePositions[agent] * 0.5f; // Adjusted for closer grouping
                Vector3 adjustedPosition = communicationManager.GetAdjustedDestination(agent, targetPosition);
                agent.SetDestination(adjustedPosition);
                communicationManager.UpdateAgentDestination(agent, adjustedPosition);
            }
        }
    }

    // Calculate the centroid of all agents
    private Vector3 CalculateCentroid()
    {
        Vector3 centroid = Vector3.zero;
        foreach (NavMeshAgent agent in agents)
        {
            centroid += agent.transform.position;
        }
        return centroid / agents.Count;
    }

    // Draw gizmos in the editor to visualize the grouping radius and agent paths
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

    // Draw a wireframe circle in the editor
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