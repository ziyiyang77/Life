using UnityEngine;
using UnityEngine.UI;

public class Level5Trigger : MonoBehaviour
{
    public AudioClip level5Clip; // The audio clip to play
    public Button restartButton; // Reference to the restart button
    public GameObject player; // Reference to the player object
    public AudioSource audioSource; // Audio source for playing the clip
    public Sprite idleSprite; // The idle sprite to set when the animator is disabled

    private PlayerMovement playerMovement; // Reference to the player movement script
    private Animator playerAnimator; // Reference to the player's animator
    private SpriteRenderer playerSpriteRenderer; // Reference to the player's SpriteRenderer
    private bool hasPlayedMusic;

    private void Start()
    {
        if (audioSource == null || level5Clip == null)
        {
            return;
        }

        // Get the player movement, animator, and sprite renderer components
        playerMovement = player.GetComponent<PlayerMovement>();
        playerAnimator = player.GetComponent<Animator>();
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();

        // Ensure the restart button is initially disabled
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
        }

        hasPlayedMusic = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player triggered the event
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.currentLevel = "Level5";
            PlayLevel5Clip();
        }
    }

    private void PlayLevel5Clip()
    {
        if (audioSource != null && level5Clip != null && !hasPlayedMusic)
        {
            hasPlayedMusic = true;

            // Disable player movement and animator
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }

            if (playerAnimator != null)
            {
                playerAnimator.enabled = false;
            }

            // Set the idle sprite
            if (playerSpriteRenderer != null && idleSprite != null)
            {
                playerSpriteRenderer.sprite = idleSprite;
            }

            // Play the clip
            audioSource.clip = level5Clip;
            audioSource.Play();

            // Resume movement and enable restart button after clip ends
            Invoke(nameof(ResumeMovementAndEnableRestart), level5Clip.length);
        }
    }

    private void ResumeMovementAndEnableRestart()
    {
        /* // Enable player movement and animator
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (playerAnimator != null)
        {
            playerAnimator.enabled = true;
        }*/

        // Enable the restart button
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
        }
    }

}
