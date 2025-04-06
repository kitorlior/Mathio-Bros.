using System.Collections;
using UnityEngine;

public class BlockCoin : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.AddCoin();
        StartCoroutine(Animate());
    }
    private IEnumerator Animate()
    {

        Vector3 restingPosition = transform.localPosition;
        Vector3 animatedPosition = restingPosition + Vector3.up * 2f; // animate up by two blocks

        yield return Move(restingPosition, animatedPosition); // go up
        yield return Move(animatedPosition, restingPosition); // go back down

        Destroy(gameObject);
    }

    private IEnumerator Move(Vector3 from, Vector3 to) // animate change in position
    {
        float elapsed = 0f;
        float duration = 0.25f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localPosition = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;

            yield return null;
        }
        // loop might finish before reaching final position, update manually
        transform.localPosition = to;
    }
}
