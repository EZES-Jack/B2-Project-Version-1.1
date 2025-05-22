using UnityEngine;

public class ZombieSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource component
    [SerializeField] private AudioClip idleSound; // Idle zombie sound clip
    [SerializeField] private AudioClip hitSound; // Sound effect for when the player takes damage
    [SerializeField] private float soundInterval = 0f; // Interval in seconds between sounds

    private void Start()
    {
        if (audioSource != null && idleSound != null)
        {
            StartCoroutine(PlayIdleSound());
        }
    }

    private System.Collections.IEnumerator PlayIdleSound()
    {
        while (true)
        {
            float randomInterval = Random.Range(1f, 120f); // Generate a random interval between 1 and 120 seconds
            yield return new WaitForSeconds(randomInterval); // Wait for the random interval
            audioSource.PlayOneShot(idleSound); // Play the idle sound
        }
    }

    public void PlayHitSound()
    {
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound); // Play the hit sound
        }
    }

    public void PlayIdleSoundOnce()
    {
        if (audioSource != null && idleSound != null)
        {
            audioSource.PlayOneShot(idleSound); // Play the idle sound once
        }
    }

    public void StopAllSounds()
    {
        if (audioSource != null)
        {
            audioSource.Stop(); // Stop all currently playing sounds
        }
    }
}