using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]
public class AgentFollowPlayer : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject player;
    [SerializeField] private float radius = 0.4f;
    [SerializeField] private float distance = 0.4f;
    [SerializeField] private float enemyRadius = 0.4f;
    [SerializeField] private float enemyDistance = 0.4f;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player object not found!");
        }
    }

    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.transform.position);
        }

        agent.isStopped = (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, distance, LayerMask.GetMask("Player")) 
                           || Physics.SphereCast(transform.position, enemyRadius, transform.forward, out RaycastHit hit2, enemyDistance, LayerMask.GetMask("Enemy")));
    }

    public void OnTriggerEnter(Collider collision)
    {
        Debug.Log("test");
        if (collision.gameObject.CompareTag("Player"))
        {
            agent.isStopped = true;
            Debug.Log("Collision with Player detected!");
        }
    }
}