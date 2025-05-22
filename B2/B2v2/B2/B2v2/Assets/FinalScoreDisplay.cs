using UnityEngine;
using UnityEngine.UI;

public class FinalScoreDisplay : MonoBehaviour
{
    public Text finalScoreText;

    void Start()
    {
        if (finalScoreText != null)
        {
            int finalScore = PlayerPrefs.GetInt("FinalScore", 0); // Retrieve the saved score
            finalScoreText.text = "Final Score: " + finalScore;
        }
    }
}