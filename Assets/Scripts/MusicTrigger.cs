using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    public AudioSource audioSource; // Reference to the audio source for the music trigger
    public AudioClip musicClip; // The audio clip to play for this trigger
    public AudioSource bgmAudioSource; // Reference to the BGM audio source
    public AudioClip bgmClip; // The BGM clip to play (optional, not every trigger will have one)
    private bool hasTriggered = false; // Flag to ensure the trigger only activates once

    public GameObject player; // Reference to the player object
    public Sprite idleSprite; // The idle sprite to set when the animator is disabled

    private PlayerMovement playerMovement; // Reference to the player movement script
    private Animator playerAnimator; // Reference to the player's animator
    private SpriteRenderer playerSpriteRenderer; // Reference to the player's SpriteRenderer

    private BoxCollider2D triggerCollider; // Reference to the trigger collider

    public bool isHint = false;
    public bool isNotLimitPos = false;

    private void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        playerAnimator = player.GetComponent<Animator>();
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        triggerCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; // Set the flag to true
            PlayMusic();
            UpdateBGM(); // Update BGM if a new clip is specified
        }
    }

    public void PlayMusic()
    {
        if (audioSource != null && musicClip != null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop(); // Stop any currently playing music
            }

            // Constrain the player's x position within the trigger bounds
            if (player != null && triggerCollider != null && !isNotLimitPos)
            {
                Vector3 playerPosition = player.transform.position;
                float leftLimit = triggerCollider.bounds.min.x;
                float rightLimit = triggerCollider.bounds.max.x;

                // Constrain player x within the bounds
                playerPosition.x = Mathf.Clamp(playerPosition.x, leftLimit, rightLimit);
                player.transform.position = playerPosition;
            }

            if (!isHint)
            {
                // Disable player movement and animator
                if (playerMovement != null)
                {
                    playerMovement.SetSpeed(0);
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
            }

            audioSource.clip = musicClip;
            audioSource.Play();

            Invoke(nameof(ResumeMovementAndEnableRestart), musicClip.length);
        }
    }

    public void UpdateBGM()
    {
        if (bgmAudioSource != null && bgmClip != null)
        {
            // If a new BGM clip is specified, update and play it
            bgmAudioSource.clip = bgmClip;
            bgmAudioSource.Play();
        }
    }

    private void ResumeMovementAndEnableRestart()
    {
        // Enable player movement and animator
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (playerAnimator != null)
        {
            playerAnimator.enabled = true;
        }
    }
}
