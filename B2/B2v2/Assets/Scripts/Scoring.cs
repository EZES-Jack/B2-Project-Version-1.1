// Example for Enemy.cs
using UnityEngine;

public class Scoring : MonoBehaviour
{
    public int standardPoints = 10;
    public int variantPoints = 20;

    void Die()
    {
        if (CompareTag("Variant"))
        {
            ScoreManager.instance.AddScore(variantPoints);
        }
        else
        {
            ScoreManager.instance.AddScore(standardPoints);
        }
        Destroy(gameObject);
    }
}