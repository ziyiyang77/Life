using UnityEngine;

public class StreetPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed for left/right movement

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Left/Right movement
        float moveInput = Input.GetAxis("Horizontal"); // Gets input from left/right arrow keys or A/D keys
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    public void SetSpeed(float speed)
    {
        rb.velocity *= speed;
    }
}
