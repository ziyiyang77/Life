using UnityEngine;

public class Level4Trigger : LevelTriggerBase
{
    public GameObject bulletPrefab; // Bullet prefab to spawn
    public Transform spawnArea; // Area from which bullets will rain
    public float spawnRate = 0.5f; // Rate at which bullets spawn
    public float bulletFallSpeed = 5f; // Speed at which bullets fall
    public int bossHealth = 100; // Health of the boss
    public Transform bossTransform; // Reference to the boss
    public Transform playerTransform; // Reference to the player
    public float playerAttackRange = 2f; // Range within which the player can attack the boss
    public int playerAttackDamage = 10; // Damage dealt by the player per attack
    public PlayerHealth playerHealth; // Reference to the player's health script

    public GameObject leftBorder; // Left boundary collider
    public GameObject rightBorder; // Right boundary collider
    public Cinemachine.CinemachineVirtualCamera virtualCamera; // Virtual Camera
    public GameObject pauseObject; // Pause object displayed when boss is defeated
    public GameObject HeartUI; // UI to display player health
    public GameObject boss;
    public GameObject TriggerBlock;

    public AudioSource bulletSfxSource; // Audio source for bullet spawn sounds
    public AudioClip bulletSfxClip; // Sound effect for bullet spawn
    public AudioSource attackSfxSource; // Audio source for attack sound effect
    public AudioClip attackSfxClip; // Sound effect for player attack

    private void Start()
    {
        levelTriggerCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.currentLevel = "Level4";

            // Activate level-specific features
            leftBorder.SetActive(true);
            rightBorder.SetActive(true);
            virtualCamera.Follow = null;
            HeartUI.SetActive(true);
            levelTriggerCollider.enabled = false;
            TriggerBlock.SetActive(false);

            InvokeRepeating(nameof(SpawnBullet), 0f, spawnRate); // Start bullet rain
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsLevelActive("Level4"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Animator animator = playerTransform.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                }

                // Play attack sound effect
                PlayAttackSFX();

                float distanceToBoss = Vector2.Distance(playerTransform.position, bossTransform.position);
                if (distanceToBoss <= playerAttackRange)
                {
                    BossTakeDamage(playerAttackDamage);
                }
            }
        }
    }

    private void SpawnBullet()
    {
        // Get the BoxCollider2D component from the spawnArea
        BoxCollider2D spawnAreaCollider = spawnArea.GetComponent<BoxCollider2D>();
        if (spawnAreaCollider == null)
        {
            Debug.LogError("No BoxCollider2D attached to the spawnArea!");
            return;
        }

        // Calculate the left and right edges of the spawn area
        float leftEdge = spawnArea.position.x - (spawnAreaCollider.size.x * spawnArea.localScale.x / 2);
        float rightEdge = spawnArea.position.x + (spawnAreaCollider.size.x * spawnArea.localScale.x / 2);

        // Random position within the horizontal bounds of the spawn area
        float xPosition = Random.Range(leftEdge, rightEdge);

        // Use the spawn area's Y position for vertical placement
        Vector3 spawnPosition = new Vector3(xPosition, spawnArea.position.y, spawnArea.position.z);

        // Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        // Set the bullet's downward velocity
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.down * bulletFallSpeed;
        }

        // Play the bullet spawn sound effect
        PlayBulletSFX();

        // Destroy the bullet after some time to clean up
        Destroy(bullet, 5f);
    }

    private void PlayBulletSFX()
    {
        if (bulletSfxSource != null && bulletSfxClip != null)
        {
            bulletSfxSource.PlayOneShot(bulletSfxClip); // Play the bullet sound effect
        }
    }

    private void PlayAttackSFX()
    {
        if (attackSfxSource != null && attackSfxClip != null)
        {
            attackSfxSource.PlayOneShot(attackSfxClip); // Play the attack sound effect
        }
    }

    private void BossTakeDamage(int damage)
    {
        bossHealth -= damage;
        Debug.Log($"Boss Health: {bossHealth}");

        if (bossHealth <= 0)
        {
            Debug.Log("Boss Defeated!");
            CancelInvoke(nameof(SpawnBullet)); // Stop bullet rain

            // Display pause object and deactivate level features
            pauseObject.SetActive(true);
            leftBorder.SetActive(false);
            rightBorder.SetActive(false);
            HeartUI.SetActive(false);
            if (boss != null)
            {
                Animator bossAnimator = boss.GetComponent<Animator>();
                if (bossAnimator != null)
                {
                    bossAnimator.speed = 0; // Pause the animation
                }
            }
            virtualCamera.Follow = playerTransform;
        }
    }

    public override void RestartLevel()
    {
        // Stop bullet rain
        CancelInvoke(nameof(SpawnBullet));

        // Reset boss health
        bossHealth = 100;

        TriggerBlock.SetActive(true);
        // Reset level-specific features
        leftBorder.SetActive(false);
        rightBorder.SetActive(false);
        HeartUI.SetActive(false);

        // Reset boss state
        if (boss != null)
        {
            Animator bossAnimator = boss.GetComponent<Animator>();
            if (bossAnimator != null)
            {
                bossAnimator.speed = 1; // Resume the animation if paused
            }
        }

        // Reset player-camera follow
        virtualCamera.Follow = playerTransform;

        // Reset level trigger
        ResetLevelTrigger();

        Debug.Log("Level4 has been reset.");
    }
}
