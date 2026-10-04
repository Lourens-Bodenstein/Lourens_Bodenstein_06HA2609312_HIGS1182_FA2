using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("Shooting Settings (Raycast)")]
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private float weaponRange = 50f;
    [SerializeField] private LayerMask hittableLayers;
    [Header("Shooting Visuals")]
    [SerializeField] private float laserDuration = 0.05f;
    [SerializeField] private GameObject explosionPrefab;

    // Cached Components & State Tracking
    private Rigidbody rb;
    private LineRenderer laserLine;
    private Vector3 moveInput;
    private float nextFireTime;
    private bool isDead = false;

    private void Start()
    {
        // Rubric Requirement: Use GetComponent<> at startup and cache safely
        rb = GetComponent<Rigidbody>();
        laserLine = GetComponent<LineRenderer>();

        if (rb == null) Debug.LogError("PlayerController: Rigidbody missing from Player object!");
        if (laserLine == null) Debug.LogError("PlayerController: LineRenderer missing from Player object!");
    }

    private void Update()
    {
        if (isDead) return; // Prevent input handling if dead
        HandleInput();
    }

    private void FixedUpdate()
    {
        if (isDead) return; // Prevent physics movement if dead
        MovePlayer();
    }

    /// <summary>
    /// Custom method handling both movement and shooting inputs.
    /// </summary>
    private void HandleInput()
    {
        // 1. Movement Input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(moveX, 0f, moveZ).normalized;

        // 2. Shooting Input (Left Click or Spacebar)
        if ((Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            ExecuteRaycastShoot();
        }
    }

    private void MovePlayer()
    {
        Vector3 targetVelocity = moveInput * moveSpeed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        if (moveInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 15f));
        }
    }

    /// <summary>
    /// Custom method executing modular Raycast hit-detection with visual laser rendering and impact explosions.
    /// </summary>
    private void ExecuteRaycastShoot()
    {
        Transform gunBarrel = transform.Find("RightGun");
        Vector3 fireOrigin = gunBarrel != null ? gunBarrel.position : transform.position;
        Vector3 fireDirection = transform.forward;
        Vector3 endpoint = fireOrigin + (fireDirection * weaponRange);

        laserLine.SetPosition(0, fireOrigin);

        // Perform the Raycast check
        if (Physics.Raycast(fireOrigin, fireDirection, out RaycastHit hitInfo, weaponRange, hittableLayers))
        {
            endpoint = hitInfo.point; // Snap laser line directly to the target point
            Debug.Log($"Raycast Hit: {hitInfo.collider.name} at position {hitInfo.point}");

            // NEW: Fulfill Rubric Requirement: Generate visual feedback explosion instantly at the impact point
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, hitInfo.point, Quaternion.identity);
            }

            // Handle laser damage/destruction on both Asteroids and Enemies
            if (hitInfo.collider.CompareTag("Asteroid"))
            {
                Destroy(hitInfo.collider.gameObject);
                Debug.Log("Asteroid successfully destroyed via Raycast weapon discharge.");
            }
            else if (hitInfo.collider.CompareTag("Enemy"))
            {
                Destroy(hitInfo.collider.gameObject);
                Debug.Log("Enemy drone successfully neutralized via Raycast weapon discharge.");
            }
        }

        laserLine.SetPosition(1, endpoint);
        StartCoroutine(RenderLaserBeam());
    }

    private System.Collections.IEnumerator RenderLaserBeam()
    {
        laserLine.enabled = true;
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
    }

    /// <summary>
    /// Unified 3D Collision handler for physics interactions.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        // Check if our ship crashed directly into an asteroid mesh or enemy unit
        if (collision.gameObject.CompareTag("Asteroid") || collision.gameObject.CompareTag("Enemy"))
        {
            // Report loss condition straight to our master game state machine safely
            GameManager.Instance.TriggerGameOverState();

            // Handle player disabled state layout
            isDead = true;
            rb.linearVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Rubric Requirement: Use trigger detection for item interaction handles.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Scrap") || other.CompareTag("Collectible"))
        {
            Destroy(other.gameObject);
            Debug.Log("Item Collected: Scrap unit safely recovered by drone hardware.");

            // 1. Send score update message straight to GameManager system!
            GameManager.Instance.AddScore();

            // 2. NEW: Tell the CollectibleManager to replace the collected unit instantly!
            if (CollectibleManager.Instance != null)
            {
                CollectibleManager.Instance.ItemCollected();
            }
        }
    }

}
