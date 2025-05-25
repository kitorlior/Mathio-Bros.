using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    //do we need world and stage??
    public int world {  get; private set; }
    public int stage { get; private set; }

    public string levelName;

    //TODO: decide if we want lives and coins - maybe just reset unless player specifically quits to main menu?
    public int lives { get; private set; }
    public int coins { get; private set; }
    public event System.Action<int> OnLivesChanged;

    private void Awake()
{
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }

    Instance = this;
    DontDestroyOnLoad(gameObject);
}
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private IEnumerator Start()
    {
         if (lives <= 0) // Only call NewGame once
        {
            NewGame(); // Initializes lives and coins and loads the level
        }  
        yield return null;      
        UpdateLivesUI();      
    }
    private void UpdateLivesUI()
    {
        TMP_Text livesText = GameObject.Find("Lives Text")?.GetComponent<TMP_Text>();
        if (livesText != null)
        {
            livesText.text = lives.ToString();
            Debug.Log("Lives updated to: " + lives);
        }

        // Notify any listeners
        OnLivesChanged?.Invoke(lives);
    }   

    private void NewGame()
    {        
        //LoadLevel(1, 1);        
        lives = 3;
        //LoadLevel(1, 1);
        SceneManager.LoadScene(levelName);
        coins = 0;

        if (SceneManager.GetActiveScene().name != "Level")
        {
            SceneManager.LoadScene("Level");
        }
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
        UpdateLivesUI(); // Update UI immediately

        if (lives > 0)
        {
            SceneManager.LoadScene(levelName); // need to change to handle multiple levels
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
        ReturnToMainMenu();


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
        UpdateLivesUI(); // Update UI when gaining a life
    }

    public void ReturnToMainMenu()
    {
        lives = 3; // Reset only when quitting to menu
        SceneManager.LoadScene("MainMenu");
    }
}
