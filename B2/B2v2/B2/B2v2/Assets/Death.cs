using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour
{
    [SerializeField] private Damage playerDamage; // Reference to the Damage script

    void Start()
    {
        if (playerDamage != null)
        {
            playerDamage.OnPlayerDeath += HandlePlayerDeath;
        }
    }

    void HandlePlayerDeath()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    void OnDestroy()
    {
        if (playerDamage != null)
        {
            playerDamage.OnPlayerDeath -= HandlePlayerDeath;
        }
    }
}