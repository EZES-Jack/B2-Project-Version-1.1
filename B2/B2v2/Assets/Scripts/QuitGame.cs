using System;
using UnityEngine;
using UnityEngine.UI;

public class QuitGame : MonoBehaviour
{
    [SerializeField] private Button yourButton; // Reference to the button

    void Start()
    {
        if (yourButton != null)
        {
            yourButton.onClick.AddListener(OnButtonClick);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();}
    }

    void OnButtonClick()
    {
        Application.Quit();
    }

    public void Quit()
    {
        Application.Quit();
    }
}