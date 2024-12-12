using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string currentLevel; // Name or identifier for the current level
    public bool isRestarted; // Tracks if the level has been restarted
    public string RestartSceneName;

    private Dictionary<string, Vector3> respawnPoints = new Dictionary<string, Vector3>(); // Dynamic registration

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist between scenes

        isRestarted = false; // Initialize as false
    }

    public bool IsLevelActive(string levelName)
    {
        return currentLevel == levelName;
    }

    public void RegisterRespawnPoint(string levelName, Vector3 respawnPoint)
    {
        if (respawnPoints.ContainsKey(levelName))
        {
            respawnPoints[levelName] = respawnPoint;
        }
        else
        {
            respawnPoints.Add(levelName, respawnPoint);
        }
    }

    public Vector3 GetRespawnPoint()
    {
        if (respawnPoints.TryGetValue(currentLevel, out Vector3 point))
        {
            return point;
        }
        Debug.LogWarning($"No respawn point set for level {currentLevel}. Defaulting to (0, 0, 0).");
        return Vector3.zero;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(RestartSceneName);
    }
}
