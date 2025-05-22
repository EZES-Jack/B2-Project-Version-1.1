using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; // Reference to the enemy prefab
    [SerializeField] private float spawnInterval = 5f; // Time interval between spawns
    [SerializeField] private List<string> possibleTagnames; // List of possible tagnames
    [SerializeField] private Vector3 cubeCenter; // Center of the cube
    [SerializeField] private Vector3 cubeSize; // Size of the cube
    [SerializeField] private float spawnDistance = 5f; // Distance around the spawner to spawn enemies
    [SerializeField] private int maxZombies = 25; // Maximum number of zombies per spawner
    [SerializeField] private float spawnerHealth = 100f; // Health of the spawner
    [SerializeField] private float healthLossPerZombie = 10f; // Health loss per zombie death
    [SerializeField] private int zombiesOnDestroy = 10; // Number of zombies to spawn when the spawner is destroyed
    [SerializeField] private TMP_Text healthText; // Reference to the UI Text element

    private List<GameObject> spawnedZombies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnEnemy());
        UpdateHealthText(); // Initialize the health text
    }

    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Check if the number of spawned zombies is less than the maximum allowed
            if (spawnedZombies.Count < maxZombies)
            {
                // Generate a random position around the spawner
                Vector3 spawnPosition = GetRandomPositionAroundSpawner();

                // Instantiate the enemy at the random position
                GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                spawnedZombies.Add(enemyInstance);

                // Assign a random tagname from the list
                string randomTagname = possibleTagnames[Random.Range(0, possibleTagnames.Count)];
                enemyInstance.GetComponent<ZombieAI>().Tagname = randomTagname;

                // Subscribe to the enemy's OnDestroy event
                enemyInstance.GetComponent<ZombieAI>().OnDestroyEvent += OnZombieDestroyed;

                Debug.Log("Spawned enemy with Tagname: " + randomTagname);
            }
        }
    }

    private Vector3 GetRandomPositionAroundSpawner()
    {
        Vector3 randomPosition;
        Vector3 halfSize = cubeSize / 2;
        float minDistance = 1f;

        do
        {
            // Generate a random position around the spawner within the specified distance
            randomPosition = new Vector3(
                transform.position.x + Random.Range(-spawnDistance, spawnDistance),
                transform.position.y,
                transform.position.z + Random.Range(-spawnDistance, spawnDistance)
            );
        }
        while (IsInsideOrTooCloseToCube(randomPosition, halfSize, minDistance));

        return randomPosition;
    }

    private bool IsInsideOrTooCloseToCube(Vector3 position, Vector3 halfSize, float minDistance)
    {
        return position.x > cubeCenter.x - halfSize.x - minDistance && position.x < cubeCenter.x + halfSize.x + minDistance &&
               position.y > cubeCenter.y - halfSize.y - minDistance && position.y < cubeCenter.y + halfSize.y + minDistance &&
               position.z > cubeCenter.z - halfSize.z - minDistance && position.z < cubeCenter.z + halfSize.z + minDistance;
    }

    private void OnZombieDestroyed(GameObject zombie)
    {
        spawnedZombies.Remove(zombie);
        spawnerHealth -= healthLossPerZombie;
        UpdateHealthText(); // Update the health text
        Debug.Log("Spawner health: " + spawnerHealth);

        if (spawnerHealth <= 0)
        {
            for (int i = 0; i < zombiesOnDestroy; i++)
            {
                Vector3 spawnPosition = GetRandomPositionAroundSpawner();
                GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                spawnedZombies.Add(enemyInstance);

                string randomTagname = possibleTagnames[Random.Range(0, possibleTagnames.Count)];
                enemyInstance.GetComponent<ZombieAI>().Tagname = randomTagname;

                enemyInstance.GetComponent<ZombieAI>().OnDestroyEvent += OnZombieDestroyed;

                Debug.Log("Spawned enemy with Tagname: " + randomTagname);
            }

            // Unsubscribe from all zombies' OnDestroyEvent before destroying the spawner
            foreach (var spawnedZombie in spawnedZombies)
            {
                if (spawnedZombie != null)
                {
                    spawnedZombie.GetComponent<ZombieAI>().OnDestroyEvent -= OnZombieDestroyed;
                }
            }

            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        spawnerHealth -= damage;
        UpdateHealthText(); // Update the health text
        Debug.Log("Spawner health: " + spawnerHealth);

        if (spawnerHealth <= 0)
        {
            // Unsubscribe from all zombies' OnDestroyEvent before destroying the spawner
            foreach (var spawnedZombie in spawnedZombies)
            {
                if (spawnedZombie != null)
                {
                    spawnedZombie.GetComponent<ZombieAI>().OnDestroyEvent -= OnZombieDestroyed;
                }
            }

            Destroy(gameObject);
        }
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "Spawner Health: " + spawnerHealth;
        }
    }
}