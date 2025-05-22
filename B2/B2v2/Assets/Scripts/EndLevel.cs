using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    private bool hasFirstEnemySpawned = false;

    private void Update()
    {
        // Find all objects with the "Enemy" tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Check if the first enemy has spawned
        if (!hasFirstEnemySpawned && enemies.Length > 0)
        {
            hasFirstEnemySpawned = true;
        }

        // If the first enemy has spawned and no enemies are left, load the "LevelComplete" scene
        if (hasFirstEnemySpawned && enemies.Length == 0)
        {
            SceneManager.LoadScene("LevelComplete");
        }
    }
}