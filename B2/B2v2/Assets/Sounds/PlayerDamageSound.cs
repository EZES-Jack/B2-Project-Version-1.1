using UnityEngine;

public class PlayerDamageSound : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f; // Maximum health of the player
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource component
    [SerializeField] private AudioClip damageSound; // Sound to play when the player takes damage
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth; // Initialize health
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage; // Reduce health
        Debug.Log("Player took damage: " + damage + ", Current health: " + currentHealth);

        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound); // Play the damage sound
        }
        else
        {
            Debug.LogWarning("AudioSource or damageSound is not assigned.");
        }

        if (currentHealth <= 0)
        {
            Die(); // Handle player death
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // Add death logic here (e.g., game over screen, respawn, etc.)
    }
}