using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // For switching scenes if needed

public class CountdownTimer : MonoBehaviour
{
    public float startTime = 30f; // Starting time in seconds
    private float timeRemaining;
    public Text timerText; // Reference to a UI Text component for displaying the timer

    private bool timerIsRunning = false;

    private void Start()
    {
        timeRemaining = startTime;
        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime; // Reduce time
                UpdateTimerDisplay();
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;

                // Optional: Trigger scene switch or game over actions
                // SceneManager.LoadScene("GameOverScene");
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        // Format the timer as minutes:seconds
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


}
