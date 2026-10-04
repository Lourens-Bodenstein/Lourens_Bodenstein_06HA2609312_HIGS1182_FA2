using UnityEngine;

public class AsteroidsSpawner : MonoBehaviour
{
    [Header("Spawning Hazards (Rubric: Include Enemy & Asteroid)")]
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Area Dimensions")]
    [SerializeField] private Vector3 spawnAreaCenter = Vector3.zero;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(40f, 0f, 40f);

    [Header("Spawner Settings & Adjustable Parameters")]
    [SerializeField] private float initialSpawnInterval = 2f;
    [SerializeField] private int initialMaxHazards = 10;

    [Header("Spawn Odds (Percentage)")]
    [Range(0f, 100f)][SerializeField] private float enemySpawnChance = 20f;

    // Runtime state tracking variables
    private float currentSpawnInterval;
    private int currentMaxHazards;
    private float nextSpawnTime;

    // ADD THIS EXACT LINE HERE TO FIX THE CS0103 ERROR
    private int currentAsteroidCount = 0;

    private void Start()
    {
        // Set initial difficulty states at engine runtime start
        currentSpawnInterval = initialSpawnInterval;
        currentMaxHazards = initialMaxHazards;

        // Force the first spawn to happen quickly on start
        nextSpawnTime = Time.time + 0.5f;

        // FORCE TEST: Execute immediate layout spawns on frame one
        Debug.Log("FORCE TEST: Attempting to spawn 3 items instantly on frame one!");
        SpawnObstacle();
        SpawnObstacle();
        SpawnObstacle();
    }

    private void Update()
    {
        // Continuous game-loop runtime checking using your clean integer tracker
        if (Time.time >= nextSpawnTime && currentAsteroidCount < currentMaxHazards)
        {
            nextSpawnTime = Time.time + currentSpawnInterval;
            SpawnObstacle();
        }
    }

    public void SpawnObstacle()
    {
        float randomX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2f, spawnAreaCenter.x + spawnAreaSize.x / 2f);
        float randomZ = Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2f, spawnAreaCenter.z + spawnAreaSize.z / 2f);
        Vector3 spawnPosition = new Vector3(randomX, 0f, randomZ);

        GameObject prefabToSpawn = asteroidPrefab;
        string hazardType = "Asteroid";

        float randomRoll = Random.Range(0f, 100f);
        if (randomRoll <= enemySpawnChance)
        {
            if (enemyPrefab != null)
            {
                prefabToSpawn = enemyPrefab;
                hazardType = "Enemy Drone";
            }
            else
            {
                Debug.LogWarning("Spawner Warning: Rolled for an Enemy Drone, but the 'Enemy Prefab' slot in the Inspector is EMPTY! Defaulting to Asteroid.");
            }
        }

        if (prefabToSpawn == null)
        {
            Debug.LogError("SPAWNER CRITICAL ERROR: Both prefab reference slots are completely missing from the Inspector! Cannot instantiate object.");
            return;
        }

        GameObject spawnedObj = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        spawnedObj.name = $"{hazardType}(Clone)";

        // This line will compile perfectly now!
        currentAsteroidCount++;
        Debug.Log($"Spawner Success: Instantiated {spawnedObj.name} successfully at position {spawnPosition}");
    }

    public void UpdateDifficulty(int waveMultiplier)
    {
        currentMaxHazards = initialMaxHazards + (waveMultiplier * 3);
        currentSpawnInterval = Mathf.Max(0.5f, initialSpawnInterval - (waveMultiplier * 0.4f));
        Debug.Log($"Spawner Difficulty Scaled: Max Hazards = {currentMaxHazards}, Spawn Interval = {currentSpawnInterval}s");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}
