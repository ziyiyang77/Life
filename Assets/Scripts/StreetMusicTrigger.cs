using UnityEngine;

public class StreetMusicTrigger : MonoBehaviour
{
    public AudioSource audioSource; // Reference to an existing AudioSource in the scene
    public AudioClip musicClip; // The audio clip to play when triggered
    public AudioSource bgmAudioSource; // Reference to the BGM AudioSource
    public AudioClip bgmClip; // The BGM clip to play after music ends (optional)
    public AudioClip fightingClip; // The BGM clip to switch to after musicClip ends

    private bool hasTriggered = false; // Flag to ensure the trigger only activates once

    public GameObject player; // Reference to the player object
    public Sprite idleSprite; // The idle sprite to set when the animator is disabled

    private StreetPlayerMovement playerMovement; // Reference to the player movement script
    private Animator playerAnimator; // Reference to the player's animator
    private SpriteRenderer playerSpriteRenderer; // Reference to the player's SpriteRenderer

    private BoxCollider2D triggerCollider; // Reference to the trigger collider

    public bool isHint = false;
    public bool isNotLimitPos = false;

    private void Start()
    {
        playerMovement = player.GetComponent<StreetPlayerMovement>();
        playerAnimator = player.GetComponent<Animator>();
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        triggerCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered && audioSource != null && musicClip != null)
        {
            hasTriggered = true; // Set the flag to true
            PlayMusic();
            UpdateBGM();
        }
    }

    public void PlayMusic()
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

        // Play the music clip
        audioSource.clip = musicClip;
        audioSource.Play();

        // Schedule the fighting BGM to play after the music clip ends
        Invoke(nameof(ChangeToFightingBGM), musicClip.length);

        // Restore player controls after the music clip ends
        Invoke(nameof(ResumeMovementAndEnableRestart), musicClip.length);
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

    private void ChangeToFightingBGM()
    {
        if (bgmAudioSource != null && fightingClip != null)
        {
            // Change BGM to the fighting clip
            bgmAudioSource.clip = fightingClip;
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
