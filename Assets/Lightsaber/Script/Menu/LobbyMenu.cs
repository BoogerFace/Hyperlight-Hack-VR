using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button gameOverButton; // Assign in Inspector

    [Header("Scenes")]
    public string gameOverScene = "GameOver"; // Set your GameOver scene name here

    private void Start()
    {
        if (gameOverButton != null)
        {
            gameOverButton.onClick.AddListener(GoToGameOver);
        }
        else
        {
            Debug.LogWarning("GameOver Button not assigned in LobbyMenu!");
        }
    }

    public void GoToGameOver()
    {
        Debug.Log("Loading GameOver scene...");
        SceneManager.LoadScene(gameOverScene);
    }
}
