using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;

public class Level1Trigger : LevelTriggerBase
{
    public float levelTime = 30f; // Set the countdown time in seconds
    public Text timerText; // UI Text for the timer display
    public GameObject leftBorder; // Left border collider GameObject
    public GameObject rightBorder; // Right border collider GameObject
    public CinemachineVirtualCamera virtualCamera; // Reference to the Cinemachine Virtual Camera
    private Transform playerTransform; // Reference to the player's transform

    public BulletShooter shooter1; // Reference to the first bullet shooter
    public BulletShooter shooter2; // Reference to the second bullet shooter

    private float timeRemaining;
    private bool timerIsRunning = false;

    public GameObject boss; // Reference to boss1 GameObject
    public GameObject pauseObject; // Reference to the pause object (e.g., an icon or indicator)

    public GameObject HeartUI;
    public GameObject TriggerBlock;

    public PlayerHealth playerHealth;

    private void Start()
    {
        timeRemaining = levelTime;
        timerText.gameObject.SetActive(false); // Hide timer initially
        leftBorder.SetActive(false); // Disable borders initially
        rightBorder.SetActive(false); 

        // Set playerTransform to the player's Transform (assuming player is tagged "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        shooter1.isShooting = false;
        shooter2.isShooting = false;
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();

                // Start the first shooter when the level starts
                if (timeRemaining <= levelTime && !shooter1.isShooting)
                {
                    shooter1.StartShooting();
                }

                // Start the second shooter when timer reaches 20 seconds
                if (timeRemaining <= 20 && !shooter2.isShooting)
                {
                    shooter2.StartShooting();
                }
            }
            else
            {
                shooter1.StopShooting();
                shooter2.StopShooting();
                timeRemaining = 0;
                timerIsRunning = false;

                EndLevel();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.currentLevel = "Level1";
            TriggerBlock.SetActive(false);
            StartLevel();
        }
    }

    private void StartLevel()
    {
        timeRemaining = levelTime;
        levelTriggerCollider.enabled = false;
        HeartUI.SetActive(true);
        timerText.gameObject.SetActive(true);
        timerIsRunning = true;

        leftBorder.SetActive(true);
        rightBorder.SetActive(true);

        // Remove the follow target to "freeze" the camera in place
        virtualCamera.Follow = null;
        playerHealth.currentHealth = playerHealth.maxHealth;
    }

    public void EndLevel()
    {
        StartCoroutine(DelayFollow());
        timerIsRunning = false;
        leftBorder.SetActive(false);
        rightBorder.SetActive(false);
        HeartUI.SetActive(false);

        // Pause boss animation
        if (boss != null)
        {
            Animator bossAnimator = boss.GetComponent<Animator>();
            if (bossAnimator != null)
            {
                bossAnimator.speed = 0; // Pause the animation
            }
        }

        // Activate the pause object above boss1
        if (pauseObject != null && boss != null)
        {
            pauseObject.SetActive(true);
        }
    }

    public override void RestartLevel()
    {
        shooter1.StopShooting();
        shooter2.StopShooting();
        TriggerBlock.SetActive(true);
        timeRemaining = 0;
        timerIsRunning = false;
        leftBorder.SetActive(false);
        rightBorder.SetActive(false);
        HeartUI.SetActive(false);
        timerText.gameObject.SetActive(false);
        timerIsRunning = false;
        virtualCamera.Follow = playerTransform;

    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(Mathf.Max(0, timeRemaining) / 60); // Clamp to 0 to avoid negative time
        int seconds = Mathf.FloorToInt(Mathf.Max(0, timeRemaining) % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private IEnumerator DelayFollow()
    {
        yield return new WaitForSeconds(3.0f);
        virtualCamera.Follow = playerTransform;
    }

    private IEnumerator EnableCinemachineSmoothly()
    {
        Debug.Log("Start moving camera back smoothly to the player");

        // Detach the follow target temporarily to prevent immediate snapping
        virtualCamera.Follow = null;

        // Get the CinemachineTransposer component to access the offset
        var transposer = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
        Vector3 offset = Vector3.zero;

        if (transposer != null)
        {
            offset = transposer.m_FollowOffset; // Store the original offset
        }

        // Calculate start and target positions, considering the offset
        Vector3 startPosition = virtualCamera.transform.position;
        Vector3 targetPosition = playerTransform.position + offset;
        targetPosition.z = startPosition.z; // Maintain original z-position of the camera

        float duration = 4.0f; // Duration of the smooth move
        float elapsedTime = 0f;

        // Smoothly move the camera to the player's position over the duration
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Interpolate the camera position, applying the offset to the target position
            virtualCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        // After moving smoothly, reattach the follow target and reset the original offset
        virtualCamera.Follow = playerTransform;

        Debug.Log("Camera has smoothly moved back to follow the player");
    }


    private void ResetBoss()
    {
        if (boss != null)
        {
            Animator bossAnimator = boss.GetComponent<Animator>();
            if (bossAnimator != null)
            {
                bossAnimator.speed = 1; // Resume animation
            }
        }

        if (pauseObject != null)
        {
            pauseObject.SetActive(false); // Deactivate the pause object
        }
    }


}
