using UnityEngine;
using UnityEngine.AI;
using System;

public class ZombieAI : MonoBehaviour
{
    [Header("Zombie AI Settings")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float rotationSpeed = 2f;
    public string Tagname;
    [SerializeField] private float health = 100f; // Serialized field for health

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private bool playerDetected = false;

    public event Action<GameObject> OnDestroyEvent;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(Tagname).transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.radius = 0.5f;
        navMeshAgent.height = 2.0f;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (!playerDetected)
            {
                playerDetected = true;
                detectionRange = 500f; // Increase detection range to ensure the zombie doesn't lose the player
            }

            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            navMeshAgent.SetDestination(player.position);
        }
        else if (playerDetected)
        {
            navMeshAgent.SetDestination(player.position); // Continue following the player even if out of initial range
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                health -= bullet.GetDamage();

                if (health <= 0)
                {
                    // Add points to the score when the zombie is killed
                    ScoreManager.instance?.AddScore(10);
                    OnDestroyEvent?.Invoke(gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }
}