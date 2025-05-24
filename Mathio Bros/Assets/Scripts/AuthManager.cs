using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class AuthManager : MonoBehaviour
{
    [Header("UI Elements")]
    public InputField IDInput;
    public InputField passwordInput;
    public InputField usernameInput; // Only for signup
    public Text messageText;

    private const string SERVER_URL = "http://your-server-ip:3000"; // Use IP, not localhost for Android/WebGL

    public void OnLoginClicked()
    {
        StartCoroutine(Login(emailInput.text, passwordInput.text));
    }

    public void OnSignupClicked()
    {
        StartCoroutine(SignUp(emailInput.text, passwordInput.text, usernameInput.text));
    }

    IEnumerator Login(string email, string password)
    {
        var json = JsonUtility.ToJson(new LoginData(email, password));
        UnityWebRequest request = new UnityWebRequest($"{SERVER_URL}/login", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        CheckUserExists(email,password)

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            messageText.text = "Login failed: " + request.error;
        }
        else
        {
            messageText.text = "Login success!";
            Debug.Log("Response: " + request.downloadHandler.text);
            // You can parse response JSON here to get user data
        }
    }

    IEnumerator SignUp(string email, string password, string username)
    {
        var json = JsonUtility.ToJson(new SignupData(email, password, username));
        UnityWebRequest request = new UnityWebRequest($"{SERVER_URL}/signup", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            messageText.text = "Signup failed: " + request.error;
        }
        else
        {
            messageText.text = "Signup success!";
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }
    // Check if username/email exists
    public IEnumerator CheckUserExists(string field, string value)
    {
        string url = $"{SERVER_URL}/checkUser?field={field}&value={value}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Check failed: " + request.error);
            }
            else
            {
                bool exists = JsonUtility.FromJson<UserExistsResponse>(request.downloadHandler.text).exists;
                Debug.Log(exists ? "User exists!" : "User does not exist.");
            }
        }
    }
}

[System.Serializable]
public class UserExistsResponse
{
    public bool exists;
}

    [System.Serializable]
    public class LoginData
    {
        public string username;
        public string password;
        public LoginData(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }

    [System.Serializable]
    public class SignupData
    {
        public int ID;
        public string password;
        public string username;
        public SignupData(int ID, string password, string username)
        {
            this.ID = ID;
            this.password = password;
            this.username = username;
        }
    }
}
