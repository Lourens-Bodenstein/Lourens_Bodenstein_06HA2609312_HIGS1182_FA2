using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    // Singleton pattern for quick, safe cross-script tracking
    public static CollectibleManager Instance { get; private set; }

    [Header("Item Assets")]
    [SerializeField] private GameObject collectiblePrefab;

    [Header("Spawn Layout")]
    [SerializeField] private Vector3 spawnAreaCenter = Vector3.zero;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(35f, 0f, 35f);
    [SerializeField] private int maxItemsInScene = 5; // The number of scrap items always kept alive

    private int activeItemCount = 0;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        // Populate the arena with initial salvage at initialization
        SpawnInitialCollectibles();
    }

    /// <summary>
    /// Custom method to handle initial runtime collectible placement.
    /// </summary>
    private void SpawnInitialCollectibles()
    {
        for (int i = 0; i < maxItemsInScene; i++)
        {
            SpawnSingleCollectible();
        }

        Debug.Log($"CollectibleManager: Distributed {activeItemCount} fuel canisters across space bounds.");
    }

    /// <summary>
    /// Custom method to calculate coordinates and instantiate a single scrap item.
    /// </summary>
    public void SpawnSingleCollectible()
    {
        float randomX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2f, spawnAreaCenter.x + spawnAreaSize.x / 2f);
        float randomZ = Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2f, spawnAreaCenter.z + spawnAreaSize.z / 2f);

        // Match your vertical position (Y coordinate) exactly to your player height level
        Vector3 spawnPosition = new Vector3(randomX, 0.5f, randomZ);

        if (collectiblePrefab != null)
        {
            Instantiate(collectiblePrefab, spawnPosition, Quaternion.identity);
            activeItemCount++;
        }
    }

    /// <summary>
    /// Public method called by the player script when an item is collected.
    /// </summary>
    public void ItemCollected()
    {
        activeItemCount = Mathf.Max(0, activeItemCount - 1);

        // Instantly spawn a replacement scrap unit somewhere else in the field
        SpawnSingleCollectible();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}
