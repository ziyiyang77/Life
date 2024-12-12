using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    public float shieldDuration = 3f; // Duration the shield lasts
    public float shieldCooldown = 1f; // Cooldown time after shield expires
    public Animator animator; // Reference to the Animator
    public AudioSource audioSource; // AudioSource for shield sound effect
    public AudioClip shieldSFX; // Sound effect for activating the shield

    private bool isShieldActive = false;
    private bool canActivateShield = true;
    private float shieldTimer = 0f;

    private void Update()
    {
        if (GameManager.Instance.IsLevelActive("Level2"))
        {
            if (Input.GetKeyDown(KeyCode.E) && canActivateShield)
            {
                ActivateShield();
            }
        }

        if (isShieldActive)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0)
            {
                DeactivateShield();
            }
        }
    }

    private void ActivateShield()
    {
        Debug.Log("Shield activated");
        isShieldActive = true;
        canActivateShield = false;
        shieldTimer = shieldDuration;

        // Set the Animator's defense parameter to true
        if (animator != null)
            animator.SetBool("Defense", true);

        // Make the player invulnerable
        gameObject.layer = LayerMask.NameToLayer("Invulnerable");

        // Play the shield activation sound effect
        PlayShieldSFX();
    }

    private void DeactivateShield()
    {
        isShieldActive = false;
        shieldTimer = shieldCooldown;

        // Set the Animator's defense parameter to false
        if (animator != null)
            animator.SetBool("Defense", false);

        // Revert the player to the default layer to make them vulnerable again
        gameObject.layer = LayerMask.NameToLayer("Default");

        // Start cooldown timer before allowing shield activation again
        Invoke(nameof(ResetShieldCooldown), shieldCooldown);
    }

    private void ResetShieldCooldown()
    {
        canActivateShield = true;
    }

    private void PlayShieldSFX()
    {
        if (audioSource != null && shieldSFX != null)
        {
            audioSource.PlayOneShot(shieldSFX); // Play the sound effect
        }
    }
}
