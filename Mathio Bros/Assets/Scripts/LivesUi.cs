using TMPro;
using UnityEngine;

public class LivesUI : MonoBehaviour
{
    private TMP_Text livesText;

    private void Awake()
    {
        livesText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        // Subscribe to GameManager's event
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLivesChanged += UpdateLives;
            UpdateLives(GameManager.Instance.lives); // Show current lives
        }
    }

    private void UpdateLives(int lives)
    {
        livesText.text = lives.ToString();
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLivesChanged -= UpdateLives;
        }
    }
}
