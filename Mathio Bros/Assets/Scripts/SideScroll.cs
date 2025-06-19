using Photon.Pun;
using UnityEngine;

public class SideScroll : MonoBehaviourPunCallbacks
{
    private Transform player;
    private Transform[] playerTransforms;
    private Vector3 lastPosition;

    public Vector3 resetPosition = new Vector3(12.5f, 7f, -10f);

    public float height = 7f;
    public float undergroundHeight = -9f;

    private void Start()
    {
        
        if (!GameManager.Instance.isMulti)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        if (GameManager.Instance.isMulti)
        {
            UpdatePlayerReferences();
            UpdateCameraPosition();
        }
        else
        {
            Vector3 cameraPosition = transform.position;
            cameraPosition.x = Mathf.Max(cameraPosition.x, player.position.x);
            transform.position = cameraPosition; // camera cannot go back left - player cannot go back
        }
    }

    public void SetUnderground(bool underground) // if player goes over/underground via pipe - set camera accordingly
    {
        Vector3 cameraPosition = transform.position;
        cameraPosition.y = underground ? undergroundHeight : height;
        transform.position = cameraPosition;
    }

    public void SetUnderground(bool underground, float x)
    {
        Vector3 cameraPosition = transform.position;
        cameraPosition.x = x;
        cameraPosition.y = underground ? undergroundHeight : height;
        transform.position = cameraPosition;
        Debug.Log("camera:" + cameraPosition.ToString());
    }

    private void UpdatePlayerReferences()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        playerTransforms = new Transform[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playerTransforms[i] = players[i].transform;
        }
    }

    private void UpdateCameraPosition()
    {
        if (playerTransforms == null || playerTransforms.Length == 0) return;

        float maxPlayerX = float.MinValue;
        foreach (Transform player in playerTransforms)
        {
            if (player.position.x > maxPlayerX)
            {
                maxPlayerX = player.position.x;
            }
        }

        Vector3 cameraPosition = transform.position;
        cameraPosition.x = Mathf.Max(cameraPosition.x, maxPlayerX);
    }

    public void Reset()
    {
        Debug.Log("reset camera");
        transform.position = resetPosition;
    }
}