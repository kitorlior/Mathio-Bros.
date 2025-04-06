using System.Collections;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public KeyCode enterKeyCode = KeyCode.S;
    public Vector3 enterDirection = Vector3.down;
    public Vector3 exitDirection = Vector3.zero;
    public Transform connection;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (connection != null && collision.CompareTag("Player"))
        {
            if (Input.GetKey(enterKeyCode))
            {
                StartCoroutine(Enter(collision.transform));// start enter animation when entering pipe
            }
        }
    }

    private IEnumerator Enter(Transform player)
    {
        player.GetComponent<PlayerMovement>().enabled = false; // stop player movement

        Vector3 enteredPosition = transform.position + enterDirection; // final position
        Vector3 enteredScale = Vector3.one * 0.5f; // final scale

        yield return Move(player, enteredPosition, enteredScale); // animate entering the pipe
        yield return new WaitForSeconds(1f); // wait before moving to next location

        bool underground = connection.position.y < 0f; // check if undergroun
        Camera.main.GetComponent<SideScroll>().SetUnderground(underground); // move camera accordingly

        if (exitDirection != Vector3.zero) // if exiting a pipe - handle exit animation
        {
            player.position = connection.position - exitDirection;
            yield return Move(player, connection.position + exitDirection, Vector3.one);
        }
        else // return player to normal scale and move to next location
        {
            player.position = connection.position;
            player.localScale = Vector3.one;
        }

        player.GetComponent<PlayerMovement>().enabled = true; // resume player movement
    }

    private IEnumerator Move(Transform player, Vector3 endPosition, Vector3 endScale) // animate player size and position
    {
        float elapsed = 0f;
        float duration = 1f;

        Vector3 startPosition = player.position;
        Vector3 startScale = player.localScale;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            player.position = Vector3.Lerp(startPosition, endPosition, t); // change position according to percent of change
            player.localScale = Vector3.Lerp(startScale, endScale, t); // change scale according to percent of change

            elapsed += Time.deltaTime; // update time elapsed

            yield return null;
        }
        // loop might not finish at precise end location and scale, update manually at end
        player.position = endPosition;
        player.localScale = endScale;

    }
}
