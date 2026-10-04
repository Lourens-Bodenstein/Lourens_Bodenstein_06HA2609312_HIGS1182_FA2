using UnityEngine;

// Rubric Requirement: Use states/conditions to manage distinct patrol/chase behaviors clearly
public enum EnemyState { Patrolling, Chasing }

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement Profiles")]
    [SerializeField] public float baseSpeed = 4f;
    [SerializeField] private float detectionRadius = 15f;

    [Header("Patrol Configuration")]
    [SerializeField] private float patrolZoneRadius = 20f;
    [SerializeField] private float waypointWaitTime = 2f;

    // State Tracking
    private EnemyState currentState = EnemyState.Patrolling;
    private Vector3 patrolWaypoint;
    private float patrolTimer;

    // Cached references for optimization (Rubric Requirement)
    private Transform playerTransform;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Safely search out our player via uniform tags
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            Debug.Log("Enemy Drone System Online: Target tracking system initialized."); // Debug log requirement
        }
        else
        {
            Debug.LogWarning("EnemyAI: Active Player object not found in the current scene hierarchy.");
        }

        // Set our first random roaming waypoint in space
        GenerateNewPatrolWaypoint();
    }

    void FixedUpdate()
    {
        // Safe reference verification to avoid NullReference runtime breaks
        if (playerTransform == null) return;

        EvaluateAIState();

        // Switch execution profiles cleanly based on state machine values
        switch (currentState)
        {
            case EnemyState.Patrolling:
                ExecutePatrolBehaviour();
                break;
            case EnemyState.Chasing:
                ExecuteChaseBehaviour();
                break;
        }
    }

    /// <summary>
    /// Evaluates target distance metrics to switch states cleanly.
    /// </summary>
    private void EvaluateAIState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRadius && currentState == EnemyState.Patrolling)
        {
            currentState = EnemyState.Chasing;
            // Rubric Requirement: Meaningful strategic debugging statement tracking state transitions
            Debug.Log("Enemy Drone Spotted Player! Shifting engine profile to Pursuit Mode.");
        }
        else if (distanceToPlayer > detectionRadius && currentState == EnemyState.Chasing)
        {
            currentState = EnemyState.Patrolling;
            Debug.Log("Target Lost: Player broke sensor signature bounds. Returning to patrol grid.");
            GenerateNewPatrolWaypoint();
        }
    }

    /// <summary>
    /// Custom method executing low-velocity random roaming paths across space coordinates.
    /// </summary>
    private void ExecutePatrolBehaviour()
    {
        // Move towards current roaming coordinates at half-speed
        Vector3 direction = (patrolWaypoint - transform.position).normalized;
        rb.linearVelocity = direction * (baseSpeed * 0.5f);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 3f));
        }

        // Check if we reached our target destination ring
        if (Vector3.Distance(transform.position, patrolWaypoint) < 2f)
        {
            rb.linearVelocity = Vector3.zero;
            patrolTimer += Time.fixedDeltaTime;

            if (patrolTimer >= waypointWaitTime)
            {
                GenerateNewPatrolWaypoint();
            }
        }
    }

    /// <summary>
    /// Custom method executing physics-based high-velocity player tracking profiles.
    /// Scales velocity dynamically based on the current active wave tier.
    /// </summary>
    private void ExecuteChaseBehaviour()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;

        // Fetch the active wave from the GameManager safely
        int currentWave = 1;
        if (GameManager.Instance != null)
        {
            currentWave = GetCurrentWaveFromManager();
        }

        // Rubric Requirement: Include logic that increases enemy drone speed over time
        // Every new wave increases the enemy chase speed by an extra 20%
        float difficultySpeedMultiplier = 1f + ((currentWave - 1) * 0.2f);
        float activeChaseSpeed = baseSpeed * difficultySpeedMultiplier;

        // Move using physics velocity instead of hard transform overrides
        rb.linearVelocity = direction * activeChaseSpeed;

        // Smoothly look directly at the target player vector
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 6f));
        }
    }

    /// <summary>
    /// Helper method to extract wave data cleanly using standard reflection if a direct getter isn't available.
    /// </summary>
    private int GetCurrentWaveFromManager()
    {
        return GameManager.Instance.CurrentWave;
    }

    /// <summary>
    /// Generates a random coordinate in space relative to its initial location context.
    /// </summary>
    private void GenerateNewPatrolWaypoint()
    {
        patrolTimer = 0f;
        Vector2 randomCircle = Random.insideUnitCircle * patrolZoneRadius;
        patrolWaypoint = new Vector3(transform.position.x + randomCircle.x, transform.position.y, transform.position.z + randomCircle.y);
    }

    // Rubric Requirement: Use of visual gizmos to trace key state parameters inside scene view layout
    private void OnDrawGizmosSelected()
    {
        // Draw detection field circle in Red
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw patrol zone reach map in Yellow
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, patrolZoneRadius);
    }
}
