using UnityEngine;

public class BulletShooter : MonoBehaviour
{
    public GameObject bulletPrefab; // Reference to the bullet prefab
    public Sprite[] Sprites; // Array of word sprites for bullets
    public float fireRate = 1.0f; // Time between shots (in seconds)
    public float bulletSpeed = 5f; // Speed of the bullet
    public bool isShooting = false; // Controls when the shooter starts shooting

    public AudioSource sfxAudioSource; // AudioSource for sound effects
    public AudioClip bulletClip; // Sound effect clip for bullet spawn

    private void Start()
    {
        // Start shooting if the shooter is active from the start
        if (isShooting)
        {
            InvokeRepeating(nameof(ShootBullet), 0f, fireRate);
        }
    }

    private void ShootBullet()
    {
        // Instantiate a new bullet
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // Assign a random sprite to the bullet
        SpriteRenderer spriteRenderer = bullet.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && Sprites.Length > 0)
        {
            Sprite randomSprite = Sprites[Random.Range(0, Sprites.Length)];
            spriteRenderer.sprite = randomSprite;
        }

        // Set the bullet's velocity to move left
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.left * bulletSpeed;
        }

        // Play the bullet sound effect
        PlayBulletSFX();
    }

    private void PlayBulletSFX()
    {
        if (sfxAudioSource != null && bulletClip != null)
        {
            sfxAudioSource.PlayOneShot(bulletClip); // Play the sound effect once
        }
    }

    public void StartShooting()
    {
        // If the shooter is not shooting, start the firing sequence
        if (!isShooting)
        {
            isShooting = true;
            InvokeRepeating(nameof(ShootBullet), 0f, fireRate);
        }
    }

    public void StopShooting()
    {
        // Stop shooting
        isShooting = false;
        CancelInvoke(nameof(ShootBullet));
    }
}
