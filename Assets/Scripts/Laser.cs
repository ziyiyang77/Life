using UnityEngine;

public class Laser : MonoBehaviour
{
    public Transform playerTransform; // Player reference to target
    public PlayerHealth playerHealth; // Reference to player's health or death script
    public float laserDuration = 3f; // Duration for which laser is active
    public float laserRestTime = 2f; // Rest time between activations
    public LineRenderer lineRenderer; // Reference to LineRenderer for the laser
    public float laserExtendSpeed = 10f; // Speed at which the laser grows toward the player
    public AudioSource audioSource; // AudioSource for laser sound effect
    public AudioClip laserSFX; // Sound effect clip for the laser

    private bool isLaserActive = false;
    private float laserTimer = 0f;
    private bool isExtending = false;
    private Vector3 targetPosition;
    public float extraDistance = 2f;

    private void Start()
    {
        lineRenderer.enabled = false; // Start with the laser disabled
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.3f;
    }

    private void Update()
    {
        laserTimer -= Time.deltaTime;

        if (isLaserActive)
        {
            if (laserTimer <= 0)
            {
                // Deactivate laser and reset timer for the rest period
                lineRenderer.enabled = false;
                isLaserActive = false;
                laserTimer = laserRestTime;
                isExtending = false; // Stop extending when laser deactivates
            }
            else if (isExtending)
            {
                // Gradually extend the laser towards the player and check for hit
                ExtendLaser();
            }
            if (!isExtending)
            {
                CheckForPlayerHit();
            }
        }
        else
        {
            if (laserTimer <= 0)
            {
                // Activate laser, start extending, and reset timer for the active period
                lineRenderer.enabled = true;
                isLaserActive = true;
                laserTimer = laserDuration;
                isExtending = true;
                targetPosition = playerTransform.position
                                 + (playerTransform.position - transform.position).normalized * extraDistance; // Extend beyond the player
                StartLaserAtOrigin(); // Initialize the laser's starting point
            }
        }
    }

    private void StartLaserAtOrigin()
    {
        // Set the laser starting point at the shooter’s position with zero length
        lineRenderer.SetPosition(0, transform.position); // Origin
        lineRenderer.SetPosition(1, transform.position); // Start with no length

        // Play laser activation sound effect
        PlayLaserSFX();
    }

    private void PlayLaserSFX()
    {
        if (audioSource != null && laserSFX != null)
        {
            audioSource.PlayOneShot(laserSFX); // Play the laser sound effect once
        }
    }

    private void ExtendLaser()
    {
        // Get the current endpoint of the laser
        Vector3 currentEndPosition = lineRenderer.GetPosition(1);

        // Move the endpoint toward the target position over time
        Vector3 newEndPosition = Vector3.MoveTowards(currentEndPosition, targetPosition, laserExtendSpeed * Time.deltaTime);
        lineRenderer.SetPosition(1, newEndPosition);

        // Stop extending if the endpoint has reached the target position
        if (Vector3.Distance(newEndPosition, targetPosition) < 0.1f)
        {
            isExtending = false; // Stop extending once fully reached
        }
    }

    private void CheckForPlayerHit()
    {
        if (playerTransform == null || playerHealth == null)
            return;

        // Check if the player is on the "Invulnerable" layer
        int invulnerableLayer = LayerMask.NameToLayer("Invulnerable");
        if (playerTransform.gameObject.layer == invulnerableLayer)
        {
            Debug.Log("Player is invulnerable, no damage applied.");
            return;
        }

        // Calculate the distance from the laser to the player
        Vector3 laserStartPosition = lineRenderer.GetPosition(0);
        Vector3 laserEndPosition = lineRenderer.GetPosition(1);

        // Find the closest point on the laser line to the player
        Vector3 playerPosition = playerTransform.position;
        Vector3 closestPoint = Vector3.Project(playerPosition - laserStartPosition, laserEndPosition - laserStartPosition) + laserStartPosition;

        // Check if the closest point is within a small range of the player's position
        float hitThreshold = 0.5f; // Adjust this value based on player size and precision
        if (Vector3.Distance(closestPoint, playerPosition) <= hitThreshold)
        {
            Debug.Log("Player Die!"); // Instantly kill the player
            playerHealth.currentHealth = 0;
            playerHealth.Die();
        }
    }
}
