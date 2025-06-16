using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }

    public string levelName;
    public int Lives { get; private set; }
    public int Coins { get; private set; }
    public event System.Action<int> OnLivesChanged;
    public bool isMulti = false;

    static bool flag = false;

    private void Awake()
    {
        if (isMulti)
        {
            if (Instance == null)
            {
                Instance = this;
                PhotonView.Get(this).ViewID = 1; // Ensure consistent view ID
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Lives = 3;
        Coins = 0;
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
        yield return null;
        UpdateLivesUI();
    }

    private void UpdateLivesUI()
    {
        TMP_Text livesText = GameObject.Find("Lives Text")?.GetComponent<TMP_Text>();
        if (livesText != null)
        {
            livesText.text = Lives.ToString();
            Debug.Log("Lives updated to: " + Lives);
        }
        OnLivesChanged?.Invoke(Lives);
    }

    [PunRPC]
    public void UpdateLives(int lives)
    {
        Lives = lives;
        Debug.Log($"Lives: {lives}");
        UpdateLivesUI();
    }

    private void NewGame()
    {
        Lives = 3;
        Coins = 0;

        if (!isMulti)
        {
            SceneManager.LoadScene(levelName);
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(levelName);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MultiLevel");
        }
    }

    public void ResetLevel(float delay)
    {
        Invoke(nameof(ResetLevel), delay);
    }

    [PunRPC]
    public void ResetLevel()
    {
        Debug.Log("Restarting level");
        if (!isMulti) Lives--;

        if (isMulti)
        {
            photonView.RPC("UpdateLives", RpcTarget.All, Lives-1);
        }
        ;

        if (Lives > 0)
        {
            if (!isMulti)
            {
                SceneManager.LoadScene(levelName);
            }
            else if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("TeleportAllPlayers", RpcTarget.All, new Vector3(2.5f, 2.5f, 0f));
                Debug.Log("Teleported all players");
            }
        }
        else
        {
            GameOver();
        }

        UpdateLivesUI();
    }

    public void MultiResetLevel()
    {

        if (photonView != null)
        {
            photonView.RPC("ResetLevel", RpcTarget.MasterClient);
        }
    }

    private void GameOver()
    {
        var gameOverText = GameObject.Find("Game Over Text")?.GetComponent<TMP_Text>();
        if (gameOverText != null)
        {
            gameOverText.enabled = true;
        }

        if (!isMulti)
        {
            var player = GameObject.Find("Mario")?.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.enabled = false;
            }
        }

        Invoke(nameof(ReturnToMainMenu), 2f);
    }

    public void AddCoin()
    {
        Coins++;
        if (Coins >= 100)
        {
            AddLife();
            Coins = 0;
        }
    }

    public void AddLife()
    {
        Lives++;
        UpdateLivesUI();
    }

    public void ReturnToMainMenu()
    {
        Lives = 3;
        if (isMulti)
        {
            PhotonNetwork.LeaveRoom();
        }
        SceneManager.LoadScene("MainMenu");
    }

    [PunRPC]
    public void TeleportAllPlayers(Vector3 pos)
    {

        Camera.main.GetComponent<SideScroll>().Reset();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(players.ToString() + players.Length);
        foreach (GameObject p in players)
        {
            p.transform.position = pos;
            Debug.Log("set active, pos = " + pos.ToString());
        }
        Debug.Log("teleported all players from RPC");
    }
}