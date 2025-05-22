using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CommunicationManager : MonoBehaviour
{
    private Dictionary<NavMeshAgent, Vector3> agentDestinations = new Dictionary<NavMeshAgent, Vector3>();

    public void RegisterAgent(NavMeshAgent agent)
    {
        if (!agentDestinations.ContainsKey(agent))
        {
            agentDestinations.Add(agent, agent.transform.position);
        }
    }

    public void UpdateAgentDestination(NavMeshAgent agent, Vector3 destination)
    {
        if (agentDestinations.ContainsKey(agent))
        {
            agentDestinations[agent] = destination;
        }
    }

    public Vector3 GetAdjustedDestination(NavMeshAgent agent, Vector3 proposedDestination)
    {
        foreach (var entry in agentDestinations)
        {
            if (entry.Key != agent && Vector3.Distance(entry.Value, proposedDestination) < agent.radius * 2)
            {
                // Adjust the proposed destination to avoid collision
                proposedDestination += (proposedDestination - entry.Value).normalized * agent.radius * 2;
            }
        }
        return proposedDestination;
    }
}