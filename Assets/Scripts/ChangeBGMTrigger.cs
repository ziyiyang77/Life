using UnityEngine;

public class ChangeBGMTrigger : MonoBehaviour
{
    public AudioSource bgmAudioSource; // Reference to the background music AudioSource
    public AudioClip newBGMClip; // The new BGM clip to play

    private bool hasTriggered = false; // Flag to ensure the trigger only activates once

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player and the trigger hasn't been used
        if (other.CompareTag("Player") && !hasTriggered && bgmAudioSource != null && newBGMClip != null)
        {
            hasTriggered = true; // Mark the trigger as used
            UpdateBGM(); // Change the BGM
        }
    }

    private void UpdateBGM()
    {
        if (bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop(); // Stop the current BGM
        }

        bgmAudioSource.clip = newBGMClip; // Set the new BGM clip
        bgmAudioSource.Play(); // Play the new BGM
        Debug.Log("BGM changed to: " + newBGMClip.name); // Optional debug log
    }
}
