using UnityEngine;
using TMPro;
using System;

public class ChangeText : MonoBehaviour
{
    private void Awake()
    {
        GameManager gameManager = GameManager.Instance;
        TMP_Text text = this.GetComponent<TMP_Text>();
        text.text = gameManager.lives.ToString();
    }
    
}
