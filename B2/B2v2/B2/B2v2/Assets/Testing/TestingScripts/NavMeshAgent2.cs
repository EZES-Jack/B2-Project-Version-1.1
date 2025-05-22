using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]
public class NavMeshAgent2 : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject player;
    [SerializeField] private float stoppingDistance = 1.0f;
    [SerializeField] private float checkRadius = 0.5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player object not found!");
        }

        // Set stopping distance to prevent clipping
        agent.stoppingDistance = stoppingDistance;
    }

    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.transform.position);
        }

        // Check for nearby objects to stop the agent
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, checkRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                agent.isStopped = true;
                //Debug.Log("Agent stopped due to nearby object: " + hitCollider.tag);
                return;
            }
        }
        Debug.DrawRay(transform.position, transform.forward * 1f, Color.red, 0.1f);
        
        foreach (var hitCollider in hitColliders)
        {
          Debug.DrawLine(transform.position,hitCollider.transform.position , Color.green, 0.1f);  
        }

        agent.isStopped = false;
    }

  
}