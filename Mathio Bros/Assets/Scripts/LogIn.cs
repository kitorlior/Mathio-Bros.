using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LogInPage : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text errorLbl;

    public void OnLoginClicked()
    {
        string errorString = string.Empty;
        bool emailEmpty = emailInput.text == string.Empty, passwordEmpty = passwordInput.text == string.Empty;
        if (emailEmpty || passwordEmpty )
        {
            if (emailEmpty)
            {
                Debug.Log("email empty");
                errorString += "You must enter email address to continue!\n";
            }
            if (passwordEmpty)
            {
                Debug.Log("password empty");
                errorString += "You must enter password!";
            }
            errorLbl.text = errorString;
            return;
        }

        StartCoroutine(FirebaseAPIManager.Instance.Login(
            emailInput.text,
            passwordInput.text,
            (success, message, uid) => {
                if (success)
                {
                    SaveUserId.Instance.UserId = uid;
                    Debug.Log($"Logged in as {uid}");
                    SceneManager.LoadScene("MainMenu");
                }
                else
                {
                    Debug.LogError($"Login failed: {message}");
                }
            }
        ));
    }

    public void OnForgotPasswordClicked()
    {
        Debug.Log("Forgot Password clicked");
        // Add logic to handle forgot password (e.g., send an email)
    }

    public void OnBackClicked()
    {
        SceneManager.LoadScene("LandingPage");
    }
}