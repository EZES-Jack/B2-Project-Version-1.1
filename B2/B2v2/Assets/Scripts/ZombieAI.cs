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

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(Tagname).transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.radius = 0.5f;
        navMeshAgent.height = 2.0f;
        if (navMeshAgent != null)
        {
            navMeshAgent.stoppingDistance = attackRange;
        }
    }

    void Update()
    {
        if (player == null || navMeshAgent == null) return;

        float detectionRange = Modes.CurrentEnemyRange;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (!playerDetected)
            {
                playerDetected = true;
            }

            navMeshAgent.SetDestination(player.position);

            // Rotate to face the player
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            navMeshAgent.ResetPath();
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
                    ScoreManager.instance?.AddScore(10);
                    OnDestroyEvent?.Invoke(gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }
}