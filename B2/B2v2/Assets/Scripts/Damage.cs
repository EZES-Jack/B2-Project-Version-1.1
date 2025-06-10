using UnityEngine; // Import UnityEngine namespace for Unity-specific classes and methods
using UnityEngine.UI; // Import UnityEngine.UI namespace for UI elements
using UnityEngine.SceneManagement; // Import UnityEngine.SceneManagement namespace for scene management
using System; // Import System namespace for general-purpose classes
using System.Collections.Generic; // Import System.Collections.Generic namespace for collections like HashSet and Dictionary

public class Damage : MonoBehaviour // Define the Damage class, inheriting from MonoBehaviour
{
    public event Action OnPlayerDeath; // Declare an event for player death

    [SerializeField] private float standardEnemyDamage = 10f; // Damage dealt by standard enemies
    [SerializeField] private float variantEnemyDamage = 15f; // Damage dealt by variant enemies
    [SerializeField] private Text healthText; // Reference to the UI text displaying health
    [SerializeField] private PlayerStats playerStats; // Reference to the PlayerStats script
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource component
    [SerializeField] private AudioClip damageSound; // Audio clip to play when damage is applied

    private HashSet<GameObject> enemiesInContact = new HashSet<GameObject>(); // Track enemies currently in contact
    private Dictionary<GameObject, float> nextAllowedDamageTime = new Dictionary<GameObject, float>(); // Track cooldown times for each enemy
    [SerializeField] private float damageCooldown = 1000f; // Cooldown duration for applying damage

    void Start() // Called when the script is first initialized
    {
        if (playerStats == null) // Check if playerStats is not assigned
            playerStats = FindObjectOfType<PlayerStats>(); // Find the PlayerStats component in the scene

        if (playerStats != null) // Check if playerStats is found
            playerStats.OnHealthChanged += UpdateHealthText; // Subscribe to the OnHealthChanged event

        UpdateHealthText(); // Update the health text at the start
    }

    void OnDestroy() // Called when the script is destroyed
    {
        if (playerStats != null) // Check if playerStats is assigned
            playerStats.OnHealthChanged -= UpdateHealthText; // Unsubscribe from the OnHealthChanged event
    }

    void OnTriggerEnter(Collider other) // Called when another collider enters the trigger
    {
        if (!IsEnemy(other.gameObject)) return; // Check if the object is not an enemy, exit if true
        enemiesInContact.Add(other.gameObject); // Add the enemy to the contact list
    }

    void OnTriggerExit(Collider other) // Called when another collider exits the trigger
    {
        if (!IsEnemy(other.gameObject)) return; // Check if the object is not an enemy, exit if true
        enemiesInContact.Remove(other.gameObject); // Remove the enemy from the contact list
    }

    void Update() // Called once per frame
    {
        var toRemove = new List<GameObject>(); // Create a list to store enemies to remove

        foreach (var enemy in enemiesInContact) // Iterate through enemies in contact
        {
            if (enemy == null) // Check if the enemy is null
            {
                toRemove.Add(enemy); // Add the null enemy to the removal list
                continue; // Skip to the next iteration
            }
            TryApplyDamage(enemy); // Attempt to apply damage to the enemy
        }

        foreach (var enemy in toRemove) // Iterate through enemies to remove
        {
            enemiesInContact.Remove(enemy); // Remove the enemy from the contact list
            nextAllowedDamageTime.Remove(enemy); // Remove the enemy from the cooldown dictionary
        }
    }

    private void TryApplyDamage(GameObject enemy) // Attempt to attack player
    {
        if (!nextAllowedDamageTime.ContainsKey(enemy)) // Check if the enemy is not in the cooldown dictionary
            nextAllowedDamageTime[enemy] = 0f; // Initialize the cooldown time for the enemy

        if (Time.time >= nextAllowedDamageTime[enemy]) // Check if the cooldown has expired
        {
            float damage = enemy.CompareTag("Variant") ? variantEnemyDamage : standardEnemyDamage; // Determine damage based on enemy type
            ApplyDamage(damage); // Apply the calculated damage
            nextAllowedDamageTime[enemy] = Time.time + damageCooldown; // Set the next allowed damage time
        }
    }

    private bool IsEnemy(GameObject obj) // Check if the object is an enemy
    {
        return obj.CompareTag("Enemy") || obj.CompareTag("Variant"); // Return true if the object has the "Enemy" or "Variant" tag
    }

    private void ApplyDamage(float damage) // Apply damage to the player
    {
        if (audioSource != null && damageSound != null) // Check if audioSource and damageSound are assigned
            audioSource.PlayOneShot(damageSound); // Play the damage sound

        if (playerStats != null) // Check if playerStats is assigned
        {
            playerStats.ApplyDamage((int)damage); // Apply damage to the player

            if (playerStats.health <= 0) // Check if the player's health is zero or less
            {
                SceneManager.LoadScene("Died Screen"); // Load the "Died Screen" scene
            }
        }
    }

    private void UpdateHealthText() // Update the health text UI
    {
        if (healthText != null && playerStats != null) // Check if healthText and playerStats are assigned
        {
            healthText.text = "Health: " + playerStats.health; // Update the health text with the player's current health
        }
    }
}