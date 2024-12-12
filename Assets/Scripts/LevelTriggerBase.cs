using UnityEngine;

public class LevelTriggerBase : MonoBehaviour
{
    public BoxCollider2D levelTriggerCollider;

    private void Awake()
    {
        levelTriggerCollider = GetComponent<BoxCollider2D>();
        if (levelTriggerCollider == null)
        {
            Debug.LogError($"{gameObject.name} is missing a BoxCollider2D component.");
        }
    }

    public void ResetLevelTrigger()
    {
        if (levelTriggerCollider != null)
        {
            levelTriggerCollider.enabled = true;
        }
        RestartLevel();
    }

    public virtual void RestartLevel()
    {
        Debug.Log("Base EndLevel called. Override this in level-specific classes.");
    }
}
