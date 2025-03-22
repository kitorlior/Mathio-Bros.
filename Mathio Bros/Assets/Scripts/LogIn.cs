using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogInPage : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;
    public Button forgotPasswordButton;
    public Button backButton;

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
        forgotPasswordButton.onClick.AddListener(OnForgotPasswordClicked);
        backButton.onClick.AddListener(OnBackClicked);

    }

    private void OnLoginClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        // Add authentication logic here (e.g., check against a database)
        Debug.Log($"Logging in with Username: {username}, Password: {password}");

        // Transition to the Main Menu
        SceneManager.LoadScene("MainMenu");
    }

    private void OnForgotPasswordClicked()
    {
        Debug.Log("Forgot Password clicked");
        // Add logic to handle forgot password (e.g., send an email)
    }

    private void OnBackClicked()
    {
        SceneManager.LoadScene("LandingPage");
    }
}