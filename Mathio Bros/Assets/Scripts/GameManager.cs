using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    //do we need world and stage??
    public int world {  get; private set; }
    public int stage { get; private set; }

    //TODO: decide if we want lives and coins - maybe just reset unless player specifically quits to main menu?
    public int lives { get; private set; }
    public int coins { get; private set; }

    private void Awake() // singleton
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
        TMP_Text livesText = GameObject.Find("Lives Text").GetComponent<TMP_Text>();
        livesText.text = lives.ToString();
        Debug.Log("changed lives to " + lives.ToString() + "from start");
    }

    private void NewGame()
    {
        lives = 3;
        //LoadLevel(1, 1);
        SceneManager.LoadScene("Level");
        coins = 0;
    }

    //private void LoadLevel(int world, int stage) // most likely not going to use this 
    //{
    //    this.world = world;
    //    this.stage = stage;

    //    //SceneManager.LoadScene($"{world}-{stage}"); change for naming levels and make sure scenes are named accordingly
    //}

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
            SceneManager.LoadScene("Level"); // need to change to handle multiple levels
            TMP_Text livesText = GameObject.Find("Lives Text").GetComponent<TMP_Text>();
            livesText.text = lives.ToString();
            Debug.Log("changed lives to " + lives.ToString() + "from reset level");
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        //SceneManager.LoadScene("Losing page?"); // do we even want this?

        GameObject.Find("Game Over Text").GetComponent<TMP_Text>().enabled = true;
        GameObject.Find("Mario").GetComponent<PlayerMovement>().enabled = false;


    }

    public void AddCoin()
    {
        coins++;

        if (coins == 100)
        {
            AddLife();
            coins = 0;
        }
    }

    public void AddLife()
    {
        lives++;
    }
}
