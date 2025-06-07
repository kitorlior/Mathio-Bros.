using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class AuthManager : MonoBehaviour
{
    [Header("UI Elements")]
    public InputField IDInput; // For signup
    public InputField usernameInput; // For both login and signup
    public InputField passwordInput;
    public Text messageText;

    private const string SERVER_URL = "http://your-server-ip:3000"; // Use IP, not localhost for Android/WebGL

    public void OnLoginClicked()
    {
        if (usernameInput == null || passwordInput == null)
        {
            Debug.LogError("Input fields are not assigned.");
            return;
        }
        StartCoroutine(Login(usernameInput.text, passwordInput.text));
    }

    public void OnSignupClicked()
    {
        if (IDInput == null || usernameInput == null || passwordInput == null)
        {
            Debug.LogError("Input fields are not assigned.");
            return;
        }
        int id;
        if (!int.TryParse(IDInput.text, out id))
        {
            messageText.text = "Invalid ID. Please enter a valid number.";
            return;
        }
        StartCoroutine(SignUp(id, usernameInput.text, passwordInput.text));
    }

    IEnumerator Login(string username, string password)
    {
        var json = JsonUtility.ToJson(new LoginData(username, password));
        UnityWebRequest request = new UnityWebRequest($"{SERVER_URL}/login", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            messageText.text = "Login failed: " + request.error;
            Debug.LogError("Login failed: " + request.error);
        }
        else
        {
            messageText.text = "Login success!";
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }

    IEnumerator SignUp(int id, string username, string password)
    {
        var json = JsonUtility.ToJson(new SignupData(id, username, password));
        UnityWebRequest request = new UnityWebRequest($"{SERVER_URL}/signup", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            messageText.text = "Signup failed: " + request.error;
            Debug.LogError("Signup failed: " + request.error);
        }
        else
        {
            messageText.text = "Signup success!";
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }

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
    public string username;
    public string password;
    public SignupData(int ID, string username, string password)
    {
        this.ID = ID;
        this.username = username;
        this.password = password;
    }
}
