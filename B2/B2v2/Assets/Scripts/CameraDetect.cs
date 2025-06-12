using UnityEngine;
using UnityEngine.AI;

public class CameraDetectionArea : MonoBehaviour
{
    public string playerTag = "Player";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Vector3 playerPosition = other.transform.position;

            // Find all Ai2 agents
            foreach (Ai2 ai in FindObjectsOfType<Ai2>())
            {
                if (ai.TryGetComponent<NavMeshAgent>(out var agent))
                {
                    agent.SetDestination(playerPosition);
                }
            }

            // Find all ZombieAI agents
            foreach (ZombieAI zombie in FindObjectsOfType<ZombieAI>())
            {
                if (zombie.TryGetComponent<NavMeshAgent>(out var agent))
                {
                    agent.SetDestination(playerPosition);
                }
            }
        }
    }
}