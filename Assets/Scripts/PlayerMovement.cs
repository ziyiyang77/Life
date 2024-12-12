using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed for left/right movement
    public float jumpForce = 10f; // Force for jumping

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer; // Reference to SpriteRenderer for flipping
    private bool isGrounded;

    // Ground check variables
    public Transform groundCheck; // Attach an empty GameObject to the player's feet
    public float groundCheckRadius = 0.1f; // Radius for ground check overlap
    public LayerMask groundLayer; // Define the layer that represents the ground

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Reference the Animator component
        spriteRenderer = GetComponent<SpriteRenderer>(); // Reference the SpriteRenderer component
    }

    private void Update()
    {
        // Left/Right movement
        float moveInput = Input.GetAxis("Horizontal"); // Gets input from left/right arrow keys or A/D keys
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Update Speed parameter for walking animation
        animator.SetFloat("Speed", Mathf.Abs(moveInput)); // Using Mathf.Abs to ensure Speed is positive

        // Flip the sprite based on movement direction
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false; // Facing right
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true; // Facing left
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Update IsGrounded parameter for jumping animation
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void FixedUpdate()
    {
        // Ground check using Physics2D.OverlapCircle
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void SetSpeed(float speed)
    {
        rb.velocity *= speed;
    }

}
