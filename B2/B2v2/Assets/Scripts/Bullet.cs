using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;
    [SerializeField] private float range = 50f;
    private Vector3 initialPosition;

    public void SetDamage(float damageAmount)
    {
        damage = damageAmount;
    }

    public float GetDamage()
    {
        return damage;
    }

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(initialPosition, transform.position) > range)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            return;
        }

        Destroy(gameObject);
    }
}