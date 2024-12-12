using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float maxTravelDistance = 10f; // Maximum distance the bullet can travel before being destroyed

    private Vector3 startPosition;

    private void Start()
    {
        // Record the initial position when the bullet is spawned
        startPosition = transform.position;
    }

    private void Update()
    {

        // Check if the bullet has traveled the maximum distance
        if (Vector3.Distance(startPosition, transform.position) >= maxTravelDistance)
        {
            Destroy(gameObject); // Destroy the bullet if it exceeds the maximum travel distance
        }
    }
}
