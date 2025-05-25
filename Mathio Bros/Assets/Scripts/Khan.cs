using UnityEngine;

public class Khan : MonoBehaviour
{

    public string url;
    public void OnClick()
    {
        Application.OpenURL(url);
    }
}
