using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3; // Maximum health
    public int currentHealth; // Current health

    public Sprite heartFull; // Full heart sprite
    public Sprite heartEmpty = null; // Empty heart sprite
    public List<Image> hearts; // List of heart UI Image components

    public AudioSource audioSource; // AudioSource for sound effects
    public AudioClip dieSFX; // Sound effect for player death
    public AudioClip takeDamageSFX; // Sound effect for taking damage

    private void Start()
    {
        // Initialize health and update the UI
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player collided with a bullet
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(1); // Lose 1 health
            Destroy(other.gameObject); // Destroy the bullet
        }
    }

    private void TakeDamage(int amount)
    {
        currentHealth -= amount;

        // Play take damage sound effect
        PlayTakeDamageSFX();

        UpdateHealthUI();

        // Check if health has dropped to zero or below
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Player has died");

        // Play the die sound effect
        PlayDieSFX();

        // Play the die animation
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die"); // Trigger the death animation
        }

        // Start coroutine to respawn after the animation completes
        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        // Wait for the length of the death animation
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // Assuming the death animation is on the base layer and named "Die"
            yield return new WaitForSeconds(stateInfo.length);
        }
        else
        {
            // Fallback in case no animator or animation length is available
            yield return new WaitForSeconds(1.0f);
        }

        // Respawn the player after the animation finishes
        Respawn();
    }

    private void UpdateHealthUI()
    {
        // Loop through the hearts and update their sprites based on current health
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = heartFull; // Full heart
            }
            else
            {
                hearts[i].sprite = heartEmpty; // Empty heart
            }
        }
    }

    private void Respawn()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        // Move the player to the respawn point
        transform.position = GameManager.Instance.GetRespawnPoint();

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
                else
                {
                    Debug.LogError($"LevelTriggerBase component not found on {triggerObjectName}.");
                }
            }
            else
            {
                Debug.LogError($"Level trigger object {triggerObjectName} not found in the scene.");
            }
        }
        else
        {
            Debug.LogError("Current level is not set in the GameManager.");
        }
    }

    private void PlayDieSFX()
    {
        if (audioSource != null && dieSFX != null)
        {
            audioSource.PlayOneShot(dieSFX); // Play the death sound effect
        }
    }

    private void PlayTakeDamageSFX()
    {
        if (audioSource != null && takeDamageSFX != null)
        {
            audioSource.PlayOneShot(takeDamageSFX); // Play the take damage sound effect
        }
    }
}
