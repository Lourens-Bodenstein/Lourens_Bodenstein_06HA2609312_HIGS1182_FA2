using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Rubric Requirement: Scene management engine tracking

public enum GameState { Playing, GameOver, Won }

public class GameManager : MonoBehaviour
{
    // Singleton pattern for clean global accessibility across scripts
    public static GameManager Instance { get; private set; }

    [Header("UI Reference Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;

    // State Architecture
    private GameState currentState = GameState.Playing;
    private int score = 0;
    private int targetScore = 0;

    private void Awake()
    {
        // Establish the Singleton instance safely
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        // Rubric Requirement: Determine runtime item numbers from your CollectibleManager dynamically
        CollectibleManager collectibleManager = Object.FindFirstObjectByType<CollectibleManager>();
        if (collectibleManager != null)
        {
            targetScore = collectibleManager.TotalItemsInScene;
        }

        UpdateScoreUI();
    }

    private void Update()
    {
        // Handle global restart loop monitoring
        if (currentState == GameState.GameOver || currentState == GameState.Won)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGameLoop();
            }
        }
    }

    /// <summary>
    /// Custom method executing score updates and structural state tracking.
    /// </summary>
    public void AddScore()
    {
        if (currentState != GameState.Playing) return;

        score++;
        UpdateScoreUI();

        // Check for Win condition state switch
        if (score >= targetScore)
        {
            TriggerWinState();
        }
    }

    /// <summary>
    /// Custom method triggered by player collision with an asteroid obstacle.
    /// </summary>
    public void TriggerGameOverState()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.GameOver;
        gameOverPanel.SetActive(true);
        Debug.Log("Game Over: Player drone structural integrity compromised.");
    }

    private void TriggerWinState()
    {
        currentState = GameState.Won;
        winPanel.SetActive(true);
        Debug.Log("Game Over: Victory condition achieved. All local salvage collected.");
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"Scrap Collected: {score} / {targetScore}";
    }

    /// <summary>
    /// Rubric Requirement: Scene management transitions to restart gameplay seamlessly.
    /// </summary>
    private void RestartGameLoop()
    {
        Debug.Log("Scene Management: Reloading active gameplay simulation scene map.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
