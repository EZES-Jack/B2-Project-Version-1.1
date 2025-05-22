using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Damage : MonoBehaviour
{
    public event Action OnPlayerDeath; // Event for player death

    [SerializeField] private float health = 100f; // Initial health value
    [SerializeField] private float damageAmount = 10f; // Amount of damage to apply
    [SerializeField] private Text healthText; // Reference to the UI Text element
    private float timeInContact = 0f; // Time spent in contact with the hitbox
    private bool isInContact = false; // Flag to check if in contact with the hitbox
    private GameObject enemyObject; // Reference to the enemy object

    void Start()
    {
        // Ensure the GameObject has a SphereCollider component
        SphereCollider sphereCollider = gameObject.GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            Debug.LogError("SphereCollider component is missing.");
        }

        // Ensure the SphereCollider is set as a trigger
        if (sphereCollider != null && !sphereCollider.isTrigger)
        {
            Debug.LogError("SphereCollider is not set as a trigger.");
        }

        // Ensure the GameObject has a Rigidbody component
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing.");
        }

        // Initialize the health text
        UpdateHealthText();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the "Enemy" tag
        if (other.gameObject.CompareTag("Enemy"))
        {
            ApplyDamage(); // Apply initial damage
            isInContact = true; // Set the contact flag to true
            enemyObject = other.gameObject; // Store the reference to the enemy object
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the exited object has the "Enemy" tag
        if (other.gameObject.CompareTag("Enemy"))
        {
            isInContact = false; // Set the contact flag to false
            timeInContact = 0f; // Reset the contact time
            enemyObject = null; // Clear the reference to the enemy object
        }
    }

    void Update()
    {
        // Check if the object is in contact with the hitbox
        if (isInContact)
        {
            // Ensure the enemy object still exists
            if (enemyObject == null)
            {
                isInContact = false;
                return;
            }

            timeInContact += Time.deltaTime; // Increment the contact time
            if (timeInContact >= 0.6f)
            {
                ApplyDamage(); // Apply damage after the specified interval
                timeInContact = 0f; // Reset the timer after applying damage
            }
        }
    }

    private void ApplyDamage()
    {
        health -= damageAmount; // Reduce health by the damage amount
        UpdateHealthText(); // Update the health text

        if (health <= 0)
        {
            // Handle the object's death
            ScoreManager.instance?.AddScore(10); // Add points to the score
            SceneManager.LoadScene("Died Screen"); // Load the final scene
        }
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + health;
        }
    }
}