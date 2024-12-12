using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Button quitButton; // Reference to the quit button

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            // Enable quit button if the level was restarted
            quitButton.gameObject.SetActive(GameManager.Instance.isRestarted);
        }
        else
        {
            quitButton.gameObject.SetActive(false); // Default to disabled
        }
    }
}
