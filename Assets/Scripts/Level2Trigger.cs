using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;

public class Level2Trigger : LevelTriggerBase
{
    public float levelTime = 30f;
    public Text timerText;
    public GameObject leftBorder;
    public GameObject rightBorder;
    public CinemachineVirtualCamera virtualCamera;
    private Transform playerTransform;

    public BulletShooter shooter1;
    public BulletShooter shooter2;
    public Laser laser;
    public PlayerShield playerShield;

    private float timeRemaining;
    private bool timerIsRunning = false;

    public GameObject HeartUI;
    public GameObject boss;
    public GameObject pauseObject;
    public MusicTrigger musicTrigger;
    private bool hasPlayedMusic;
    public GameObject TriggerBlock;

    public PlayerHealth playerHealth;

    private void Start()
    {
        timeRemaining = levelTime;
        timerText.gameObject.SetActive(false);
        leftBorder.SetActive(false);
        rightBorder.SetActive(false);
        levelTriggerCollider = GetComponent<BoxCollider2D>();

        // Set playerTransform to the player's Transform (assuming player is tagged "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            laser.playerTransform = playerTransform; // Pass player reference to the laser
        }

        shooter1.isShooting = false;
        shooter2.isShooting = false;
        laser.enabled = false;
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

                if (timeRemaining <= 25 && !hasPlayedMusic)
                {
                    Debug.Log("Playing Music");
                    musicTrigger.PlayMusic();
                    hasPlayedMusic = true;
                }


                // Start the second shooter when timer reaches 20 seconds
                if (timeRemaining <= 20 && !shooter2.isShooting)
                {
                    shooter2.StartShooting();
                }

                // Start laser attack when timer reaches 20 seconds
                if (timeRemaining <= 20 && !laser.enabled)
                {
                    laser.gameObject.SetActive(true);
                    laser.enabled = true;
                }
            }
            else
            {
                shooter1.StopShooting();
                shooter2.StopShooting();
                laser.enabled = false;
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
            GameManager.Instance.currentLevel = "Level2";
            TriggerBlock.SetActive(false);
            StartLevel();
        }
    }

    private void StartLevel()
    {
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

    private void EndLevel()
    {
        StartCoroutine(DelayFollow());
        leftBorder.SetActive(false);
        rightBorder.SetActive(false);
        HeartUI.SetActive(false);
        laser.gameObject.SetActive(false);

        // Pause boss animation
        if (boss != null)
        {
            Animator bossAnimator = boss.GetComponent<Animator>();
            if (bossAnimator != null)
            {
                bossAnimator.speed = 0; // Pause the animation
            }
        }

        // Activate the pause object above boss
        if (pauseObject != null && boss != null)
        {
            pauseObject.SetActive(true);
        }
    }

    public override void RestartLevel()
    {
        shooter1.StopShooting();
        shooter2.StopShooting();
        laser.enabled = false;
        laser.gameObject.SetActive(false);
        TriggerBlock.SetActive(true);
        timeRemaining = 30;
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
        int minutes = Mathf.FloorToInt(Mathf.Max(0, timeRemaining) / 60);
        int seconds = Mathf.FloorToInt(Mathf.Max(0, timeRemaining) % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private IEnumerator DelayFollow()
    {
        yield return new WaitForSeconds(3.0f);
        virtualCamera.Follow = playerTransform;
    }
}
