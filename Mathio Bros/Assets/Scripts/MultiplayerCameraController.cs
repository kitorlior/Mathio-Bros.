using UnityEngine;
using Photon.Pun;

public class MultiplayerCameraController : MonoBehaviourPunCallbacks
{

    private Transform[] playerTransforms;
    private Vector3 lastPosition;

    private void LateUpdate()
    {

        UpdatePlayerReferences();
        UpdateCameraPosition();
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

        // Only update if position changed (optimization)
        if (cameraPosition != lastPosition)
        {
            transform.position = cameraPosition;
            lastPosition = cameraPosition;
        }
    }
}