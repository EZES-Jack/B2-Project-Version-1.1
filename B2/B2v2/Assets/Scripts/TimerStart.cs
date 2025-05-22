using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerStart : MonoBehaviour
{
    private float startTime;

    void Start()
    {
        // Record the time when the scene starts
        startTime = Time.time;
    }

    void OnDisable()
    {
        // Calculate the elapsed time and save it when the scene unloads
        float elapsedTime = Time.time - startTime;
        PlayerPrefs.SetFloat("ElapsedTime", elapsedTime);
        PlayerPrefs.Save();
    }
}