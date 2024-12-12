using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StreetFighterLevelTrigger : LevelTriggerBase
{
    public Transform enemyTransform; // Reference to the enemy
    public Transform playerTransform; // Reference to the player
    public GameObject TriggerBlock; // Object to block the trigger

    public GameObject playerHealthBarUI; // Parent GameObject for player's health bar
    public Slider playerHealthBar; // Slider for player's health
    public GameObject enemyHealthBarUI; // Parent GameObject for enemy's health bar
    public Slider enemyHealthBar; // Slider for enemy's health

    public int playerMaxHealth = 100; // Max health for the player
    public int enemyMaxHealth = 100; // Max health for the enemy

    public float playerAttackRange = 2f; // Range within which the player can attack the enemy
    public int playerAttackDamage = 10; // Damage dealt by the player per attack

    public float enemyAttackIntervalMin = 2f; // Minimum interval for enemy attacks
    public float enemyAttackIntervalMax = 4f; // Maximum interval for enemy attacks
    public int enemyAttackDamage = 10; // Damage dealt by the enemy per attack
    public float enemyMoveDistance = 2f; // Distance for enemy random movement
    public float enemyMoveSpeed = 1f; // Speed of enemy movement

    public BoxCollider2D movementBoundary; // Movement boundary (assign in Inspector)

    private int playerCurrentHealth; // Current health of the player
    private int enemyCurrentHealth; // Current health of the enemy
    private bool isEnemyAttacking = false; // Track if the enemy is attacking
    private Animator playerAnimator; // Animator for the player
    private Animator enemyAnimator; // Animator for the enemy

    public GameObject leftBorder; // Left border collider GameObject
    public GameObject rightBorder; // Right border collider GameObject
    public GameObject pauseObject;

    public StreetMusicTrigger musicTrigger;


    private void Start()
    {
        playerAnimator = playerTransform.GetComponent<Animator>();
        enemyAnimator = enemyTransform.GetComponent<Animator>();

        playerCurrentHealth = playerMaxHealth;
        enemyCurrentHealth = enemyMaxHealth;

        UpdatePlayerHealthBar();
        UpdateEnemyHealthBar();

        // Ensure movementBoundary is assigned
        if (movementBoundary == null)
        {
            Debug.LogError("MovementBoundary is not assigned. Please assign it in the Inspector.");
        }

        // Hide health bars initially
        if (playerHealthBarUI != null) playerHealthBarUI.SetActive(false);
        if (enemyHealthBarUI != null) enemyHealthBarUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.currentLevel = "StreetFighter";

            TriggerBlock.SetActive(false); // Disable trigger block
            levelTriggerCollider.enabled = false; // Disable trigger collider

            StartLevel();
        }
    }

    private void StartLevel()
    {
        // Show health bars when the level starts
        if (playerHealthBarUI != null) playerHealthBarUI.SetActive(true);
        if (enemyHealthBarUI != null) enemyHealthBarUI.SetActive(true);

        leftBorder.SetActive(true);
        rightBorder.SetActive(true);

        InvokeRepeating(nameof(EnemyBehavior), 1f, Random.Range(enemyAttackIntervalMin, enemyAttackIntervalMax));
    }

    private void Update()
    {
        if (GameManager.Instance.IsLevelActive("StreetFighter"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayerAttack();
            }
        }
    }

    private void PlayerAttack()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Attack");
        }

        float distanceToEnemy = Vector2.Distance(playerTransform.position, enemyTransform.position);
        if (distanceToEnemy <= playerAttackRange)
        {
            EnemyTakeDamage(playerAttackDamage);
        }
    }

    private void EnemyBehavior()
    {
        if (isEnemyAttacking) return;

        isEnemyAttacking = true;

        if (Random.value > 0.8f)
        {
            EnemyMove(); // Smooth movement
        }
        else if (Random.value < 0.8f &&  Random.value < 0.7f)
        {
            EnemyAttack(); // Perform an attack
        }
        else
        {
            EnemyMove();
            EnemyAttack();
        }

        isEnemyAttacking = false;
    }

    private void EnemyMove()
    {
        float moveDirection = Random.value > 0.5f ? 1f : -1f; // Randomly choose forward or backward
        Vector3 targetPosition = enemyTransform.position + new Vector3(moveDirection * enemyMoveDistance, 0, 0);

        // Clamp the target position within the BoxCollider2D bounds
        if (movementBoundary != null)
        {
            Bounds bounds = movementBoundary.bounds;
            targetPosition.x = Mathf.Clamp(targetPosition.x, bounds.min.x, bounds.max.x);
        }

        // Start the coroutine to move the enemy smoothly to the target position
        StartCoroutine(MoveToPosition(enemyTransform, targetPosition, enemyMoveSpeed));
    }

    private IEnumerator MoveToPosition(Transform transform, Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null; // Wait until the next frame
        }
        transform.position = targetPosition; // Snap to the target position to avoid overshooting
    }

    private void EnemyAttack()
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("Attack");
        }

        float distanceToPlayer = Vector2.Distance(enemyTransform.position, playerTransform.position);
        if (distanceToPlayer <= playerAttackRange)
        {
            PlayerTakeDamage(enemyAttackDamage);
        }
    }

    private void PlayerTakeDamage(int damage)
    {
        playerCurrentHealth -= damage;
        UpdatePlayerHealthBar();

        if (playerCurrentHealth <= 0)
        {
            Debug.Log("Player has been defeated!");
            RestartLevel();
        }
    }

    private void EnemyTakeDamage(int damage)
    {
        enemyCurrentHealth -= damage;
        UpdateEnemyHealthBar();

        if (enemyCurrentHealth <= 0)
        {
            Debug.Log("Enemy has been defeated!");
            EndLevel();
        }
    }

    private void UpdatePlayerHealthBar()
    {
        if (playerHealthBar != null)
        {
            playerHealthBar.value = (float)playerCurrentHealth / playerMaxHealth * playerHealthBar.maxValue;
        }
    }

    private void UpdateEnemyHealthBar()
    {
        if (enemyHealthBar != null)
        {
            enemyHealthBar.value = (float)enemyCurrentHealth / enemyMaxHealth * enemyHealthBar.maxValue;
        }
    }

    public override void RestartLevel()
    {
        CancelInvoke(nameof(EnemyBehavior));

        playerCurrentHealth = playerMaxHealth;
        enemyCurrentHealth = enemyMaxHealth;

        UpdatePlayerHealthBar();
        UpdateEnemyHealthBar();

        if (playerHealthBarUI != null) playerHealthBarUI.SetActive(false);
        if (enemyHealthBarUI != null) enemyHealthBarUI.SetActive(false);

        leftBorder.SetActive(false);
        rightBorder.SetActive(false);

        //TriggerBlock.SetActive(true);

        levelTriggerCollider.enabled = true;
        Debug.Log("Level restarted.");

        playerTransform.position = GameManager.Instance.GetRespawnPoint();

        // Determine the correct level trigger based on the current level
        string currentLevel = GameManager.Instance.currentLevel;
        if (!string.IsNullOrEmpty(currentLevel))
        {
            string triggerObjectName = $"{currentLevel}Trigger"; // Naming convention: e.g., "Level1Trigger", "Level2Trigger"
            GameObject triggerObject = GameObject.Find(triggerObjectName);

            if (triggerObject != null)
            {
                var levelTrigger = triggerObject.GetComponent<LevelTriggerBase>(); // Generic or base trigger class
                if (levelTrigger != null)
                {
                    levelTrigger.ResetLevelTrigger();
                }
            }
        }
    }

    public void EndLevel()
    {
        CancelInvoke(nameof(EnemyBehavior));
        BoxCollider2D enemyCollider = enemyTransform.GetComponent<BoxCollider2D>();
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        leftBorder.SetActive(false);
        rightBorder.SetActive(false);

        if (playerHealthBarUI != null) playerHealthBarUI.SetActive(false);
        if (enemyHealthBarUI != null) enemyHealthBarUI.SetActive(false);

        musicTrigger.PlayMusic();


        enemyAnimator.speed = 0; // Pause the animation

        // Activate the pause object above boss1
        if (pauseObject != null)
        {
            pauseObject.SetActive(true);
        }
        Debug.Log("Level completed!");
        // Implement additional end logic, e.g., transition to the next level
    }
}
