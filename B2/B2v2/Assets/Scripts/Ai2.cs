using UnityEngine;

public class Ai2 : MonoBehaviour
{
    public float attackRange = 2f;
    public float rotationSpeed = 2f;
    public float moveSpeed = 3f;
    public string Tagname;
    [SerializeField] private float health = 100f;
    private Transform player;
    public bool playerDetected = false;
    public event System.Action<GameObject> OnDestroyEvent;

    private LineRenderer lineRenderer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(Tagname).transform;

        // Initialize LineRenderer for Game view
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 51; // 50 segments + 1 to close the circle
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        UpdateLineRenderer();
    }

    void Update()
    {
        if (player == null) return;

        float detectionRange = Modes.CurrentVariantRange;
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

        // Update the LineRenderer in case detection range changes
        UpdateLineRenderer();
    }

    void OnDrawGizmos()
    {
        // Draw detection range as a 2D circle on the X-axis in Scene view
        Gizmos.color = Color.red;
        float detectionRange = Modes.CurrentVariantRange;
        int segments = 50; // Number of segments for the circle
        Vector3 previousPoint = transform.position + new Vector3(detectionRange, 0, 0);
    
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 newPoint = transform.position + new Vector3(Mathf.Cos(angle) * detectionRange, 0, Mathf.Sin(angle) * detectionRange);
            Gizmos.DrawLine(previousPoint, newPoint);
            previousPoint = newPoint;
        }
    }
    
    private void UpdateLineRenderer()
    {
        if (lineRenderer == null) return;
    
        float detectionRange = Modes.CurrentVariantRange;
        int segments = 50; // Number of segments for the circle
        lineRenderer.positionCount = segments + 1;
    
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            float x = Mathf.Cos(angle) * detectionRange;
            float z = Mathf.Sin(angle) * detectionRange;
            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
        }
    }

    public void AssignRandomTagname(string randomTagname)
    {
        Tagname = randomTagname;
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