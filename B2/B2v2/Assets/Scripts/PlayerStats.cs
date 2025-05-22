using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public int health = 100;
    public int maxHealth = 100;
    public float regenDelay = 5f;      // Wait 5 seconds before starting regen
    public float regenDuration = 5f;   // Heal to full over 5 seconds

    public float damage = 10f;
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
            StopCoroutine(regenCoroutine);
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

        int startHealth = health;
        float elapsed = 0f;

        while (elapsed < regenDuration && health < maxHealth)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / regenDuration);
            health = Mathf.RoundToInt(Mathf.Lerp(startHealth, maxHealth, t));
            OnHealthChanged?.Invoke();
            yield return null;
        }

        health = maxHealth;
        OnHealthChanged?.Invoke();
    }
}