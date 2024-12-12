using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    public string levelName; // Name of the level this respawn point belongs to

    private void Start()
    {
        // Automatically register this respawn point with the GameManager at runtime
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterRespawnPoint(levelName, transform.position);
        }
        else
        {
            Debug.LogError("GameManager instance not found. Make sure it exists in the scene.");
        }
    }
}
