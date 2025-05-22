using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Spawnable
{
    public GameObject prefab;
    public float spawnRate;
}

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<Spawnable> spawnables;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private List<string> possibleTagnames;
    [SerializeField] private Vector3 cubeCenter;
    [SerializeField] private Vector3 cubeSize;
    [SerializeField] private float spawnDistance = 5f;
    [SerializeField] private int maxZombies = 25;
    [SerializeField] private float spawnerHealth = 100f;
    [SerializeField] private float healthLossPerZombie = 10f;
    [SerializeField] private int zombiesOnDestroy = 10;
    [SerializeField] private TMP_Text healthText;

    private List<GameObject> spawnedZombies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnEnemy());
        UpdateHealthText();
    }

    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (spawnedZombies.Count < maxZombies)
            {
                Vector3 spawnPosition = GetRandomPositionAroundSpawner();
                GameObject selectedPrefab = SelectRandomPrefab();

                if (selectedPrefab != null)
                {
                    GameObject enemyInstance = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
                    spawnedZombies.Add(enemyInstance);

                    string randomTagname = possibleTagnames[Random.Range(0, possibleTagnames.Count)];
                    
                    var zombieAI = enemyInstance.GetComponent<ZombieAI>();
                    if (zombieAI != null)
                    {
                        zombieAI.Tagname = randomTagname;
                        zombieAI.OnDestroyEvent += OnZombieDestroyed;
                    }
                    else
                    {
                        var ai2 = enemyInstance.GetComponent<Ai2>();
                        if (ai2 != null)
                        {
                            ai2.Tagname = randomTagname;
                            ai2.OnDestroyEvent += OnZombieDestroyed;
                        }
                    }
                }
            }
        }
    }

    private GameObject SelectRandomPrefab()
    {
        float totalRate = 0f;
        foreach (var spawnable in spawnables)
        {
            totalRate += spawnable.spawnRate;
        }

        float randomValue = Random.Range(0, totalRate);
        float cumulativeRate = 0f;

        foreach (var spawnable in spawnables)
        {
            cumulativeRate += spawnable.spawnRate;
            if (randomValue <= cumulativeRate)
            {
                return spawnable.prefab;
            }
        }

        return null;
    }

    private Vector3 GetRandomPositionAroundSpawner()
    {
        Vector3 randomPosition;
        Vector3 halfSize = cubeSize / 2;
        float minDistance = 1f;

        do
        {
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
        UpdateHealthText();
        Debug.Log("Spawner health: " + spawnerHealth);

        if (spawnerHealth <= 0)
        {
            for (int i = 0; i < zombiesOnDestroy; i++)
            {
                Vector3 spawnPosition = GetRandomPositionAroundSpawner();
                GameObject selectedPrefab = SelectRandomPrefab();

                if (selectedPrefab != null)
                {
                    GameObject enemyInstance = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
                    spawnedZombies.Add(enemyInstance);

                    string randomTagname = possibleTagnames[Random.Range(0, possibleTagnames.Count)];

                    var zombieAI = enemyInstance.GetComponent<ZombieAI>();
                    if (zombieAI != null)
                    {
                        zombieAI.Tagname = randomTagname;
                        zombieAI.OnDestroyEvent += OnZombieDestroyed;
                        Debug.Log("Spawned ZombieAI with Tagname: " + randomTagname);
                    }
                    else
                    {
                        var ai2 = enemyInstance.GetComponent<Ai2>();
                        if (ai2 != null)
                        {
                            ai2.Tagname = randomTagname;
                            ai2.OnDestroyEvent += OnZombieDestroyed;
                            Debug.Log("Spawned Ai2 with Tagname: " + randomTagname);
                        }
                    }
                }
            }

            foreach (var spawnedZombie in spawnedZombies)
            {
                if (spawnedZombie != null)
                {
                    var zombieAI = spawnedZombie.GetComponent<ZombieAI>();
                    if (zombieAI != null)
                    {
                        zombieAI.OnDestroyEvent -= OnZombieDestroyed;
                    }
                    else
                    {
                        var ai2 = spawnedZombie.GetComponent<Ai2>();
                        if (ai2 != null)
                        {
                            ai2.OnDestroyEvent -= OnZombieDestroyed;
                        }
                    }
                }
            }

            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        spawnerHealth -= damage;
        UpdateHealthText();
        Debug.Log("Spawner health: " + spawnerHealth);

        if (spawnerHealth <= 0)
        {
            foreach (var spawnedZombie in spawnedZombies)
            {
                if (spawnedZombie != null)
                {
                    var zombieAI = spawnedZombie.GetComponent<ZombieAI>();
                    if (zombieAI != null)
                    {
                        zombieAI.OnDestroyEvent -= OnZombieDestroyed;
                    }
                    else
                    {
                        var ai2 = spawnedZombie.GetComponent<Ai2>();
                        if (ai2 != null)
                        {
                            ai2.OnDestroyEvent -= OnZombieDestroyed;
                        }
                    }
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