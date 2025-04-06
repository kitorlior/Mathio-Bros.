using System.Collections;
using UnityEngine;

public class BlockItem : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Animate());
    }
    
    private IEnumerator Animate()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        CircleCollider2D physicsCollider = GetComponent<CircleCollider2D>();
        BoxCollider2D triggerCollider = GetComponent<BoxCollider2D>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // make sure nothing weird happens while animation in progress
        rb.bodyType = RigidbodyType2D.Kinematic;
        physicsCollider.enabled = false;
        triggerCollider.enabled = false;
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(0.25f);

        spriteRenderer.enabled = true; // if hidden - show
        
        //start animation loop
        float elapsed = 0f;
        float duration = 0.5f;

        Vector3 startPosition = transform.localPosition;
        Vector3 endPosition = transform.localPosition + Vector3.up;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            //move position based on progress
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
            elapsed += Time.deltaTime;

            yield return null;
        }
        // loop might end before final dest, update manually
        transform.localPosition = endPosition;

        //return to normal after finishing animation
        rb.bodyType = RigidbodyType2D.Dynamic; 
        physicsCollider.enabled = true;
        triggerCollider.enabled = true;

    }
}
