using UnityEngine;
using UnityEngine.UI;

public class TimerDisplay : MonoBehaviour
{
    public Text timerText; // Reference to the UI Text element

    void Start()
    {
        // Retrieve the saved elapsed time
        float elapsedTime = PlayerPrefs.GetFloat("ElapsedTime", 0f);

        // Convert the elapsed time to an integer
        int elapsedTimeInt = Mathf.FloorToInt(elapsedTime);

        // Display the elapsed time in the text element
        if (timerText != null)
        {
            timerText.text = $"Time: {elapsedTimeInt} seconds";
        }
    }
}