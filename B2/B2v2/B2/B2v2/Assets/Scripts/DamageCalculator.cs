using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int damageAmount = 15;
    [SerializeField] private float damageInterval = 2.0f;
    private int currentHealth;
    private Coroutine damageCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log("Player Health initialized to: " + currentHealth);
        Debug.Log("ran");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<NavMeshAgent>() != null)
        {
            Debug.Log("Collision with NavMeshAgent detected. Starting damage coroutine.");
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(ApplyDamageOverTime(collision.gameObject));
            }
        }
        else
        {
            Debug.Log("Collision with non-NavMeshAgent object detected.");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<NavMeshAgent>() != null)
        {
            Debug.Log("NavMeshAgent exited collision. Stopping damage coroutine.");
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
        else
        {
            Debug.Log("Non-NavMeshAgent object exited collision.");
        }
    }

    private IEnumerator ApplyDamageOverTime(GameObject agent)
    {
        while (true)
        {
            currentHealth -= damageAmount;
            Debug.Log("Player Health decreased to: " + currentHealth);

            if (currentHealth <= 0)
            {
                Debug.Log("Player Health has reached zero or below. Player is dead.");
                // Add any additional logic for player death here
                break;
            }

            yield return new WaitForSeconds(damageInterval);
        }
    }
}