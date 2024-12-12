using UnityEngine;

public class PlayerSwitchBoundary : MonoBehaviour
{
    public GameObject defaultPlayer; // The default player (initially active)
    public GameObject pixelPlayer; // The alternative player (switched to within the boundary)
    public GameObject smokeEffectPrefab; // Smoke effect to instantiate during the switch
    public Cinemachine.CinemachineVirtualCamera virtualCamera; // Virtual camera for the scene
    public Transform cameraFocusPoint; // Transform to reference for adjusting the camera's position

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (defaultPlayer.activeSelf)
            {
                SwitchPlayer(defaultPlayer, pixelPlayer, true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (pixelPlayer.activeSelf)
            {
                SwitchPlayer(pixelPlayer, defaultPlayer, false);
            }
        }
    }

    private void SwitchPlayer(GameObject currentPlayer, GameObject newPlayer, bool adjustCamera)
    {
        // Play smoke animation at the current player's position
        if (smokeEffectPrefab != null)
        {
            GameObject smoke = Instantiate(smokeEffectPrefab, currentPlayer.transform.position, Quaternion.identity);
            Destroy(smoke, 1f); // Destroy the smoke effect after 1 second (adjust time as per animation length)
        }

        // Disable the current player
        currentPlayer.SetActive(false);

        // Enable the new player and set its position to the current player's position
        newPlayer.transform.position = currentPlayer.transform.position;
        newPlayer.SetActive(true);

        // Adjust the camera if required
        if (adjustCamera && virtualCamera != null)
        {
            virtualCamera.Follow = null; // Detach the virtual camera from the current player

            // Adjust the camera's position to match the camera focus point
            if (cameraFocusPoint != null)
            {
                virtualCamera.transform.position = new Vector3(
                    cameraFocusPoint.position.x,
                    cameraFocusPoint.position.y,
                    virtualCamera.transform.position.z
                );
            }
        }
        else if (!adjustCamera && virtualCamera != null)
        {
            virtualCamera.Follow = defaultPlayer.transform; // Detach the virtual camera from the current player
        }

        Debug.Log($"Switched from {currentPlayer.name} to {newPlayer.name}");
    }
}
