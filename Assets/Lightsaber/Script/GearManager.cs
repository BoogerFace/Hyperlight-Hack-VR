using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GearManager : MonoBehaviour
{
    public static GearManager instance;

    public int currentGears = 0;
    public TMP_Text gearText; // will be found dynamically
    public string gameOverScene = "GameOver"; // Name of the Game Over scene

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Hook into scene loads
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Called whenever a new scene loads
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Look for a TextMeshPro UI element named "Score"
        GameObject gearObj = GameObject.Find("Score");
        if (gearObj != null)
        {
            gearText = gearObj.GetComponent<TMP_Text>();
            UpdateUI();
        }
    }

    public void AddGears(int amount)
    {
        currentGears += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (gearText != null)
        {
            gearText.text = "Gears: " + currentGears;
        }
    }

    // Called when the player dies
    public void PlayerDied()
    {
        if (currentGears >= 1000)
        {
            currentGears -= 1000; // cost to continue
            UpdateUI();

            // Heal the player to full
            PlayerHealth player = FindObjectOfType<PlayerHealth>();
            if (player != null)
            {
                player.HealToFull();
            }

            Debug.Log("Player revived using 1000 gears!");
        }
        else
        {
            Debug.Log("Not enough gears. Sending player to Game Over.");
            SceneManager.LoadScene(gameOverScene);
        }
    }

}
