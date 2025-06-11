using UnityEngine;

public class SideScroll : MonoBehaviour
{
    private Transform player;

    public float height = 7f;
    public float undergroundHeight = -9f;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        Vector3 cameraPosition = transform.position;
        cameraPosition.x = Mathf.Max(cameraPosition.x, player.position.x);
        transform.position = cameraPosition; // camera cannot go back left - player cannot go back
    }

    public void SetUnderground(bool underground) // if player goes over/underground via pipe - set camera accordingly
    {
        Vector3 cameraPosition = transform.position;
        cameraPosition.y = underground ? undergroundHeight : height;
        transform.position = cameraPosition;
    }
}