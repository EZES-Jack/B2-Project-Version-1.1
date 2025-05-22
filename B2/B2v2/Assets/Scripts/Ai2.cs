using UnityEngine;

public class Ai2 : MonoBehaviour
{
    public float detectionRange = 10f;
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
            // No attack or damage logic here
        }
    }

    public void AssignRandomTagname(string randomTagname)
    {
        Tagname = randomTagname;
        Debug.Log("Assigned Tagname to Ai2: " + Tagname);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                health -= bullet.GetDamage();

                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                if (health <= 0)
                {
                    ScoreManager.instance?.AddScore(50);
                    OnDestroyEvent?.Invoke(gameObject);
                    Destroy(gameObject);
                }
            }
        }
        else if (collision.gameObject.CompareTag("breakable"))
        {
            Destroy(collision.gameObject);
        }
    }
}