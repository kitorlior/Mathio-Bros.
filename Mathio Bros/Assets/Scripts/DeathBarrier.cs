using Photon.Pun;
using UnityEngine;

public class DeathBarrier : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) // handle death by falling
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.SetActive(false);
            if (GameManager.Instance.isMulti)
            {
                collision.gameObject.transform.position = new Vector3(2.5f, 2.5f, 0f);
                Debug.Log("Master Client? " + PhotonNetwork.IsMasterClient.ToString());
                PauseForSeconds(1f);
                collision.gameObject.SetActive(true);
                GameManager.Instance.MultiResetLevel();
            }
            else
                GameManager.Instance.ResetLevel();
        }
        else { Destroy(collision.gameObject); }
    }

    void PauseForSeconds(float seconds)
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < startTime + seconds)
        {
            // Do nothing (freezes the main thread!)
        }
        Debug.Log(seconds + " seconds passed!");
    }
}
