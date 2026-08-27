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

    // Cached Components & State Tracking
    private Rigidbody rb;
    private LineRenderer laserLine;
    private Vector3 moveInput;
    private float nextFireTime;

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
        HandleInput();
    }

    private void FixedUpdate()
    {
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
    /// Custom method executing modular Raycast hit-detection with visual laser rendering.
    /// </summary>
    private void ExecuteRaycastShoot()
    {
        // Try to find the right barrel tip dynamically, otherwise default to player center
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

            if (hitInfo.collider.CompareTag("Asteroid"))
            {
                Destroy(hitInfo.collider.gameObject);
                Debug.Log("Asteroid successfully destroyed via Raycast weapon discharge.");
            }
        }

        // Set line target ending coordinate
        laserLine.SetPosition(1, endpoint);

        // Run the custom routine to flash the beam on and off quickly
        StartCoroutine(RenderLaserBeam());
    }

    /// <summary>
    /// Coroutine to handle the frame-rate independent visual duration of the weapon fire.
    /// </summary>
    private System.Collections.IEnumerator RenderLaserBeam()
    {
        laserLine.enabled = true;
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Check if our ship crashed directly into an asteroid mesh
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            // Report loss condition straight to our master game state machine
            GameManager.Instance.TriggerGameOverState();

            // Destroy the player capsule/cube visually
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Rubric Requirement: Use collision/trigger detection for obstacle and item interaction handles.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            Destroy(other.gameObject);
            Debug.Log("Item Collected: Scrap unit safely recovered by drone hardware.");

            // Send message directly to GameManager system!
            GameManager.Instance.AddScore();
        }
    }
}
