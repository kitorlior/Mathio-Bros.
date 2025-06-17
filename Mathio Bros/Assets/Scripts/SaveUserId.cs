using UnityEngine;

public class SaveUserId : MonoBehaviour
{
    public static SaveUserId Instance { get; private set;}

    public string UserId { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    
}
