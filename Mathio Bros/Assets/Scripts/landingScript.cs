using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LandingPage : MonoBehaviour
{
    public Button loginButton;
    public Button signInButton;

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
        signInButton.onClick.AddListener(OnSignInClicked);
    }

    private void OnLoginClicked()
    {
        SceneManager.LoadScene("LogIn");
    }

    private void OnSignInClicked()
    {
        SceneManager.LoadScene("SignIn");
    }
}