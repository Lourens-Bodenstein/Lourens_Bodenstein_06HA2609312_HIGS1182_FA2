using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    [Header("Item Assets")]
    [SerializeField] private GameObject collectiblePrefab;

    [Header("Spawn Layout")]
    [SerializeField] private Vector3 spawnAreaCenter = Vector3.zero;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(35f, 0f, 35f);
    [SerializeField] private int itemsToSpawn = 5;

    // Track total spawned for score tracking
    public int TotalItemsInScene { get; private set; }

    private void Start()
    {
        // Populate the arena with salvage fuel canisters at initialization
        SpawnInitialCollectibles();
    }

    /// <summary>
    /// Custom method to handle randomized runtime collectible placement.
    /// </summary>
    private void SpawnInitialCollectibles()
    {
        for (int i = 0; i < itemsToSpawn; i++)
        {
            float randomX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2f, spawnAreaCenter.x + spawnAreaSize.x / 2f);
            float randomZ = Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2f, spawnAreaCenter.z + spawnAreaSize.z / 2f);

            Vector3 spawnPosition = new Vector3(randomX, 0.5f, randomZ);

            Instantiate(collectiblePrefab, spawnPosition, Quaternion.identity);
            TotalItemsInScene++;
        }

        Debug.Log($"CollectibleManager: Distributed {TotalItemsInScene} fuel canisters across space bounds.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}
