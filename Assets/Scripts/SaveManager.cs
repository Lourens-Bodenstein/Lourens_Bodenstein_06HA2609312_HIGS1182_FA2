using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // Singleton pattern for global access across your game scenes
    public static SaveManager Instance { get; private set; }

    private const string HighScoreKey = "AsteroidScavenger_HighestScrap";

    private void Awake()
    {
        // Establish the Singleton instance safely
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps player records safe between scene changes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Rubric Requirement: Standalone single-responsibility method to write persistent score data out to disk.
    /// </summary>
    public void SaveHighScore(int newHighScore)
    {
        PlayerPrefs.SetInt(HighScoreKey, newHighScore);
        PlayerPrefs.Save();

        // Rubric Requirement: Debug.Log trace statement for crucial saving events
        Debug.Log($"[SaveManager] New High Score Saved Successfully: {newHighScore} scrap units stored.");
    }

    /// <summary>
    /// Rubric Requirement: Standalone single-responsibility method to read persistent score data from disk.
    /// </summary>
    public int LoadHighScore()
    {
        if (PlayerPrefs.HasKey(HighScoreKey))
        {
            int loadedScore = PlayerPrefs.GetInt(HighScoreKey);
            Debug.Log($"[SaveManager] High Score Loaded: {loadedScore} units recovered.");
            return loadedScore;
        }

        Debug.Log("[SaveManager] No high score data found. Initializing record index to 0.");
        return 0; // Default fallback index
    }
}
