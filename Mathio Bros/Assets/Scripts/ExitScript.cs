using Photon.Pun.Demo.SlotRacer.Utils;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ReturnToMainMenu()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }

    public void SinglePlayer()
    {
        SceneManager.LoadScene("MainMenu");
        GameManager.Instance.mainMenu();
    }
}
