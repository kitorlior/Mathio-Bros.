using UnityEngine;
using TMPro;

public class ChangeText : MonoBehaviour
{
    private void Awake()
    {
        GameManager gameManager = GameManager.Instance;
        TMP_Text text = GetComponent<TMP_Text>();
        text.text = gameManager.Lives.ToString();
    }
    
}
