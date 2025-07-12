using Photon.Pun.Demo.SlotRacer.Utils;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitScript : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }

    public void SinglePlayer()
    {
        SceneManager.LoadScene("MainMenu");
        GameManager.Instance.MainMenu();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
