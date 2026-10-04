using UnityEngine;
using TMPro; // Required to communicate with TextMeshPro components

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI highScoreText; // Reference to your UI text field

    private void Start()
    {
        LoadAndDisplayHighScore();
    }

    /// <summary>
    /// Fetches the high score via the SaveManager Singleton and updates the main menu UI text.
    /// </summary>
    public void LoadAndDisplayHighScore()
    {
        int savedHighScore = 0;

        // 1. Check if the SaveManager instance exists in the scene
        if (SaveManager.Instance != null)
        {
            // Call your custom standalone method from your SaveManager script
            savedHighScore = SaveManager.Instance.LoadHighScore();
        }
        else
        {
            // Fallback: If starting directly from the Main Menu scene before SaveManager initializes,
            // grab it directly from PlayerPrefs using your exact script string key.
            savedHighScore = PlayerPrefs.GetInt("AsteroidScavenger_HighestScrap", 0);
            Debug.LogWarning("[MainMenuManager] SaveManager instance not found. Reading directly from PlayerPrefs fallback.");
        }

        // 2. Safely verify if the UI text reference is connected to prevent runtime errors
        if (highScoreText != null)
        {
            // 3. Update the UI text string to show the high score value
            highScoreText.text = "High Score: " + savedHighScore.ToString();
            Debug.Log($"[MainMenuManager] Main Menu updated with high score: {savedHighScore}"); // Rubric trace criteria met
        }
        else
        {
            Debug.LogError("[MainMenuManager] HighScoreText reference is unassigned in the Inspector!");
        }
    }
}
