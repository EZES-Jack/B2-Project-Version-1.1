using UnityEngine;
using UnityEngine.AI;

public class Ai2 : MonoBehaviour
{
    public float attackRange = 2f;
    public float rotationSpeed = 2f;
    public float moveSpeed = 12f;
    public string Tagname;
    [SerializeField] private float health = 100f;
    private Transform player;
    private PlayerStats playerStats;
    public bool playerDetected = false;
    public event System.Action<GameObject> OnDestroyEvent;
    private NavMeshAgent agent;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(Tagname)?.transform;
        if (player != null)
            playerStats = player.GetComponent<PlayerStats>();
    
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;
    
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = 0f; // Ensure agent gets as close as possible
        }
    }

    void Update()
    {
        if (player == null) return;

        float detectionRange = Modes.CurrentVariantRange;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!playerDetected && distanceToPlayer <= detectionRange)
        {
            playerDetected = true;
        }

       if (playerDetected)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        
            if (agent != null)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else
            {
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            float damage = 100f;
            health -= damage;
            if (health <= 0f)
            {
                Destroy(gameObject);
                OnDestroyEvent?.Invoke(gameObject);
            }
        }
        if (collision.gameObject.CompareTag("breakable"))
        {
            Destroy(collision.gameObject);
        }
    }
}