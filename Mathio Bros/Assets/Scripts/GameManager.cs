using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    public int world {  get; private set; }
    public int stage { get; private set; }
    public int lives { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        lives = 3;
        //LoadLevel(1, 1);
        SceneManager.LoadScene("Level");
    }

    private void LoadLevel(int world, int stage) // most likely not going to use this
    {
        this.world = world;
        this.stage = stage;

        //SceneManager.LoadScene($"{world}-{stage}"); change for naming levels and make sure scenes are named accordingly
    }

    //TODO decide on level names and how to save their names, maybe we need `difficulty - number`? how to save user built levels?

    public void ResetLevel(float delay)
    {
        Invoke(nameof(ResetLevel), delay);
    }

    public void ResetLevel()
    {
        lives--;
        if (lives > 0)
        {
            SceneManager.LoadScene("Level");
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        SceneManager.LoadScene("Losing page?"); // do we even want this?
    }
}
