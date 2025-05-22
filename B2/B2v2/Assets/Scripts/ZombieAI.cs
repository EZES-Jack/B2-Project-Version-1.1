using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    public float attackRange = 2f;
    public float rotationSpeed = 2f;
    public float moveSpeed = 3f;
    public string Tagname;
    [SerializeField] private float health = 100f;
    private Transform player;
    private bool playerDetected = false;
    public event System.Action<GameObject> OnDestroyEvent;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(Tagname).transform;
    }

    void Update()
    {
        if (player == null) return;

        float detectionRange = Modes.CurrentEnemyRange; // Always use Modes
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            if (!playerDetected)
            {
                playerDetected = true;
            }

            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            if (distanceToPlayer > attackRange)
            {
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
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