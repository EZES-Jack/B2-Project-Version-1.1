using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class Damage : MonoBehaviour
{
    public event Action OnPlayerDeath;

    [SerializeField] private float standardEnemyDamage = 10f;
    [SerializeField] private float variantEnemyDamage = 15f;
    [SerializeField] private Text healthText;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip damageSound;

    private HashSet<GameObject> enemiesInContact = new HashSet<GameObject>();
    private Dictionary<GameObject, float> nextAllowedDamageTime = new Dictionary<GameObject, float>();
    [SerializeField] private float damageCooldown = 1000f; // Set to 1000f for 1000 seconds

    void Start()
    {
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
            playerStats.OnHealthChanged += UpdateHealthText;
        UpdateHealthText();
    }

    void OnDestroy()
    {
        if (playerStats != null)
            playerStats.OnHealthChanged -= UpdateHealthText;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsEnemy(other.gameObject)) return;
        enemiesInContact.Add(other.gameObject);
        // Do not reset nextAllowedDamageTime here to preserve cooldown
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsEnemy(other.gameObject)) return;
        enemiesInContact.Remove(other.gameObject);
        // Do not remove from nextAllowedDamageTime to preserve cooldown
    }

    void Update()
    {
        var toRemove = new List<GameObject>();
        foreach (var enemy in enemiesInContact)
        {
            if (enemy == null)
            {
                toRemove.Add(enemy);
                continue;
            }
            TryApplyDamage(enemy);
        }
        foreach (var enemy in toRemove)
        {
            enemiesInContact.Remove(enemy);
            nextAllowedDamageTime.Remove(enemy);
        }
    }

    private void TryApplyDamage(GameObject enemy)
    {
        if (!nextAllowedDamageTime.ContainsKey(enemy))
            nextAllowedDamageTime[enemy] = 0f;

        if (Time.time >= nextAllowedDamageTime[enemy])
        {
            float damage = enemy.CompareTag("Variant") ? variantEnemyDamage : standardEnemyDamage;
            Debug.Log($"Applying {damage} damage from {enemy.name} at {Time.time}");
            ApplyDamage(damage);
            nextAllowedDamageTime[enemy] = Time.time + damageCooldown;
        }
    }

    private bool IsEnemy(GameObject obj)
    {
        return obj.CompareTag("Enemy") || obj.CompareTag("Variant");
    }

    private void ApplyDamage(float damage)
    {
        if (audioSource != null && damageSound != null)
            audioSource.PlayOneShot(damageSound);
    
        if (playerStats != null)
        {
            playerStats.ApplyDamage((int)damage);
    
            if (playerStats.health <= 0)
            {
                SceneManager.LoadScene("Died Screen");
            }
        }
    }

    private void UpdateHealthText()
    {
        if (healthText != null && playerStats != null)
        {
            healthText.text = "Health: " + playerStats.health;
        }
    }
}