using UnityEngine;
using UnityEngine.AI;
using System;

public class ZombieAI : MonoBehaviour
{
    [Header("Zombie AI Settings")]
    public float attackRange = 2f;
    public float rotationSpeed = 2f;
    public string Tagname;
    [SerializeField] private float health = 100f;
    public event Action<GameObject> OnDestroyEvent;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    public bool playerDetected = false;
    private float detectionRange;

    public float patrolRadius = 1000f; // Must be >= 30f for valid patrol points
    public float patrolPointThreshold = 0.5f;
    private Vector3 patrolPoint;
    private Vector3 spawnLocation;

    void Start()
    {
        // Use a fixed tag for the player if needed, e.g. "Player"
        player = GameObject.FindGameObjectWithTag(Tagname)?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player GameObject is tagged correctly.");
        }

        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent missing!");
            enabled = false;
            return;
        }
        navMeshAgent.radius = 0.5f;
        navMeshAgent.height = 2.0f;
        navMeshAgent.stoppingDistance = attackRange;
        navMeshAgent.isStopped = false;

        detectionRange = Modes.CurrentEnemyRange;

        spawnLocation = transform.position;
        patrolPoint = GetValidPatrolPoint();
        navMeshAgent.SetDestination(patrolPoint);
    }

    void Update()
    {
        if (player == null || navMeshAgent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (!playerDetected)
            {
                playerDetected = true;
                detectionRange = 1000f;
            }

            navMeshAgent.SetDestination(player.position);

            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (navMeshAgent.pathPending) return;

        if (Vector3.Distance(transform.position, patrolPoint) < patrolPointThreshold)
        {
            patrolPoint = GetValidPatrolPoint();
            navMeshAgent.SetDestination(patrolPoint);
        }
    }

    Vector3 GetValidPatrolPoint()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = spawnLocation + UnityEngine.Random.insideUnitSphere * patrolRadius;
            randomPos.y = spawnLocation.y;
            NavMeshHit hit;
            // Project the random point onto the NavMesh
            if (NavMesh.SamplePosition(randomPos, out hit, 10.0f, NavMesh.AllAreas))
            {
                if (Vector3.Distance(spawnLocation, hit.position) >= 30f)
                {
                    return hit.position;
                }
            }
        }
        // Fallback: return the closest NavMesh point to spawn
        NavMeshHit fallbackHit;
        if (NavMesh.SamplePosition(spawnLocation, out fallbackHit, patrolRadius, NavMesh.AllAreas))
            return fallbackHit.position;
        return spawnLocation;
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
                    ScoreManager.instance?.AddScore(10);
                    OnDestroyEvent?.Invoke(gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }
}