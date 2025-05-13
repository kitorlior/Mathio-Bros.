using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SignInPage : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public InputField ID;
    public Button signInButton;
    public Button backButton;

    private void Start()
    {
        signInButton.onClick.AddListener(OnSignInClicked);
        backButton.onClick.AddListener(OnBackClicked);
    }

    private void OnSignInClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        string id = ID.text;

        // Add sign-up logic here (e.g., save to a database)
        Debug.Log($"Signing up with Username: {username}, Password: {password}");
        
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