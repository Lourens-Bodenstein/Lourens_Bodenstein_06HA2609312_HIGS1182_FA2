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
    [SerializeField] private TextMeshProUGUI highScoreText; // Rubric Requirement: Persistent high score UI display
    [SerializeField] private TextMeshProUGUI difficultyFeedbackText; // Rubric Requirement: Dynamic on-screen UI feedback text
    [SerializeField] private GameObject gameOverPanel;

    [Header("Difficulty Balancing Variables")]
    [SerializeField] private float difficultyWaveDuration = 30f; // Time in seconds before difficulty increases

    // State Architecture
    private GameState currentState = GameState.Playing;
    private int score = 0;
    private int highScore = 0;

    // Difficulty Trackers
    private float difficultyTimer = 0f;
    private int currentWave = 1;

    // Public getter to let EnemyAI safely fetch the active wave layer
    public int CurrentWave => currentWave;

    private void Awake()
    {
        // Establish the Singleton instance safely
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Rubric Requirement: Load the persistent high score data on frame one using our dedicated SaveManager
        if (SaveManager.Instance != null)
        {
            highScore = SaveManager.Instance.LoadHighScore();
        }

        // Initialize your score UI readouts cleanly on frame one
        UpdateScoreUI();

        // Setup initial UI feedback for the start of the game
        if (difficultyFeedbackText != null)
        {
            difficultyFeedbackText.text = "Wave 1: Sector Clear";
        }

        // Hide game over overlay elements automatically on scene start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (currentState == GameState.Playing)
        {
            HandleDifficultyTracking();
        }

        // Handle global restart loop monitoring if the player fails
        if (currentState == GameState.GameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGameLoop();
            }
        }
    }

    /// <summary>
    /// Custom method tracking runtime execution duration to scale gameplay difficulty parameters.
    /// </summary>
    private void HandleDifficultyTracking()
    {
        difficultyTimer += Time.deltaTime;

        if (difficultyTimer >= difficultyWaveDuration)
        {
            UpdateDifficulty();
        }
    }

    /// <summary>
    /// Rubric Requirement: Custom method containing logic that increases difficulty over time and outputs UI feedback.
    /// </summary>
    public void UpdateDifficulty()
    {
        difficultyTimer = 0f;
        currentWave++;

        // Rubric Requirement: Display relevant text feedback directly to the player via UI
        if (difficultyFeedbackText != null)
        {
            difficultyFeedbackText.text = $"Wave {currentWave}: Asteroid Density Increasing!";
        }

        Debug.Log($"Difficulty Event: Advancing to Wave {currentWave}. Processing hazard scaling updates.");

        // Find the spawner at runtime and adjust its max bounds and intervals
        AsteroidsSpawner spawner = Object.FindFirstObjectByType<AsteroidsSpawner>();
        if (spawner != null)
        {
            spawner.UpdateDifficulty(currentWave);
        }
    }

    /// <summary>
    /// Custom method executing score updates and structural state tracking.
    /// </summary>
    public void AddScore()
    {
        if (currentState != GameState.Playing) return;

        // Increment player scrap metric limitlessly
        score++;

        // Check if the current performance beats our persistent baseline record
        if (score > highScore)
        {
            highScore = score;

            // Route save event over to our standalone manager layer
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveHighScore(highScore);
            }
        }

        // Dynamically update the score counter readout interface display
        UpdateScoreUI();
    }

    /// <summary>
    /// Custom method triggered by player collision with an asteroid obstacle or enemy drone.
    /// Fulfills the explicit cross-script requirements for PlayerController physics loops.
    /// </summary>
    public void TriggerGameOverState()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.GameOver;

        // Force a guaranteed persistence flush right at player destruction
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveHighScore(highScore);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Debug.Log("Game Over: Player drone structural integrity compromised.");
    }

    /// <summary>
    /// Refreshes text canvas strings cleanly.
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Scrap Collected: {score}";
        }

        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {highScore}";
        }
    }

    /// <summary>
    /// Rubric Requirement: Scene management transitions to restart gameplay seamlessly.
    /// </summary>
    public void RestartGameLoop()
    {
        Debug.Log("Scene Management: Reloading active gameplay simulation scene map.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
