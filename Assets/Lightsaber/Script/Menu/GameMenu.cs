using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button continueButton;
    public Button quitButton;

    [Header("Scene Settings")]
    public string lobbySceneName = "VR_Lobby"; // Change to your VR Lobby scene name


    // Start is called before the first frame update
    void Start()
    {
        // Hook up button events
        if (continueButton != null)
            continueButton.onClick.AddListener(LoadLobby);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

    }

    public void LoadLobby()
    {
        Debug.Log("> Loading VR Lobby...");
        SceneManager.LoadScene(lobbySceneName);
    }
    public void QuitGame()
    {
        Debug.Log("> Quitting Game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in editor
#else
        Application.Quit(); // Quit application in build
#endif
    }

    // Update is called once per frame
    void Update()
    {
    }
}
