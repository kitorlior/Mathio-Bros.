using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public float spawnX = 2.5f;
    public float spawnY = 2.5f;

    private void Start()
    {
        Vector2 spawnPos = new Vector2(spawnX, spawnY);
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
    }
}
