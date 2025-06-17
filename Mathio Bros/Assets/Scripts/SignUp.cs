using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text.RegularExpressions;

public class SignUp : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField emailInput;
    public TMP_Text errorLbl;

    private FirebaseAPIManager firebaseAPIManager;

    public void OnBackClicked()
    {
        SceneManager.LoadScene("LandingPage");
    }

    public void OnSignUpClicked()
    {
        bool emailEmpty = emailInput.text == "", usernameEmpty = usernameInput.text == "", passwordEmpty = passwordInput.text == "";
        string errorString = string.Empty;
        if (emailEmpty || usernameEmpty || passwordEmpty )
        {
            if (emailEmpty)
            {
                Debug.Log("email empty");
                errorString += "You must enter email address to continue!\n";
            }
            if (usernameEmpty)
            {
                Debug.Log("username empty");
                errorString += "You must enter username!\n";
            }
            if (passwordEmpty)
            {
                Debug.Log("password empty");
                errorString += "You must enter password!";
            }
            errorLbl.text = errorString;
            return;
        }
        if (!IsValidEmail(emailInput.text))
        {
            Debug.Log("invalid email");
            errorLbl.text = "Invalid email address";
            return;
        }
        StartCoroutine(FirebaseAPIManager.Instance.SignUp(
            emailInput.text,
            passwordInput.text,
            usernameInput.text,
            (success, message, uid) => {
                if (success)
                {
                    Debug.Log($"Registered as {uid}");
                    SaveUserId.Instance.UserId = uid;
                    // Save initial player data
                    StartCoroutine(FirebaseAPIManager.Instance.SavePlayerData(
                        uid,
                        1, // Starting level
                        0, // Starting score
                        0f, // Time spent
                        (saveSuccess, saveMessage) => {
                            if (saveSuccess)
                            {
                                SceneManager.LoadScene("MainMenu");
                            }
                            else
                            {
                                Debug.LogError($"Initial save failed: {saveMessage}");
                            }
                        }
                    ));
                }
                else
                {
                    Debug.LogError($"Signup failed: {message}");
                }
            }
        ));
    }
    private bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Basic structure
        return Regex.IsMatch(email, pattern);
    }

}