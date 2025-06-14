using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public float spawnX = 2.5f;
    public float spawnY = 2.5f;

    private void Start()
    {
        spawnPlayers();
    }

    public void spawnPlayers()
    {
        // Check network connection state
        if (PhotonNetwork.NetworkClientState == ClientState.Joined)
        {
            // Client is fully connected to a room
            Debug.Log($"Connected to room: {PhotonNetwork.CurrentRoom.Name}");
            Vector2 spawnPos = new Vector2(spawnX, spawnY);
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
        }
        else if (PhotonNetwork.NetworkClientState == ClientState.Joining)
        {
            Debug.Log("Currently joining a room...");
        }
        
    }
}
