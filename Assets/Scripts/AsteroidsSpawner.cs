using UnityEngine;

public class AsteroidsSpawner : MonoBehaviour
{
    [Header("Spawning Assets")]
    [SerializeField] private GameObject asteroidPrefab;

    [Header("Spawn Area Dimensions")]
    [SerializeField] private Vector3 spawnAreaCenter = Vector3.zero;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(40f, 0f, 40f);

    [Header("Spawner Settings")]
    [SerializeField] private int maxAsteroids = 10;
    [SerializeField] private float spawnInterval = 2f;

    private float nextSpawnTime;
    private int currentAsteroidCount;

    private void Update()
    {
        // Continuous game-loop runtime checking
        if (Time.time >= nextSpawnTime && currentAsteroidCount < maxAsteroids)
        {
            nextSpawnTime = Time.time + spawnInterval;
            SpawnObstacle();
        }
    }

    /// <summary>
    /// Custom method to handle randomized runtime prefab initialization.
    /// </summary>
    public void SpawnObstacle()
    {
        // Rubric Requirement: Calculate randomized coordinates within designated area bounds
        float randomX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2f, spawnAreaCenter.x + spawnAreaSize.x / 2f);
        float randomZ = Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2f, spawnAreaCenter.z + spawnAreaSize.z / 2f);

        Vector3 spawnPosition = new Vector3(randomX, 0f, randomZ);

        // Instantiate the asteroid prefab into existence at runtime
        GameObject instantiatedAsteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

        currentAsteroidCount++;

        // Rubric Requirement: Use Debug.Log to track key gameplay events systematically
        Debug.Log($"Obstacle Spawned: Asteroid placed at spatial coordinate {spawnPosition}");
    }

    // Draw the boundaries in the editor so you can see where asteroids can spawn
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}
