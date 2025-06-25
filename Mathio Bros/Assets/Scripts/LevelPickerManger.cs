using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPickerManger : MonoBehaviour
{
    public void chooseLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
