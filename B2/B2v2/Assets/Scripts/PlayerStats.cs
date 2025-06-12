using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public int health = 100;
    public int maxHealth = 100;
    public float regenDelay = 5f;      // Wait 5 seconds before starting regen
    public float healInterval = 0.1f;  // Heal 1 HP every 0.1 seconds

    public float damage = 100f;
    public float fireRate = 5f; // bullets per second
    public int ammo = 30;
    public float moveSpeed = 5f;
    public float bulletSpeed = 20f;
    public float reloadTime = 2.0f;
    public int maxAmmo = 100;
    public float minFireRate = 0.5f; // minimum bullets per second

    public delegate void HealthChanged();
    public event HealthChanged OnHealthChanged;

    private Coroutine regenCoroutine;

    void Start()
    {
        OnHealthChanged?.Invoke(); // Update UI at the start
    }

    public void ApplyDamage(int amount)
    {
        health = Mathf.Max(health - amount, 0);
        OnHealthChanged?.Invoke();

        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }
        // Start regen after delay
        regenCoroutine = StartCoroutine(HealthRegen());
    }

    public void Heal(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        OnHealthChanged?.Invoke();
    }

    private IEnumerator HealthRegen()
    {
        yield return new WaitForSeconds(regenDelay);

        while (health < maxHealth)
        {
            health = Mathf.Min(health + 1, maxHealth);
            OnHealthChanged?.Invoke();
            yield return new WaitForSeconds(healInterval);
        }
        regenCoroutine = null;
    }
}