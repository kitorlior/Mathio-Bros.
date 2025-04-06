using System.Collections;
using UnityEngine;

public class DeathAnimation : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite deadSprite;

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        UpdateSprite();
        DisablePhysics();
        StartCoroutine(Animate());
    }

    private void UpdateSprite() // change sprite and bring to front
    {
        spriteRenderer.enabled = true;
        spriteRenderer.sortingOrder = 10;

        if (deadSprite != null)
        {
            spriteRenderer.sprite = deadSprite;
        }

    }

    private void DisablePhysics() // handle disable physics for both player and other entities
    {
        Collider2D[] colliders = GetComponents<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        EntitiyMovement entitiyMovement = GetComponent<EntitiyMovement>();

        if (playerMovement != null) { playerMovement.enabled = false; }
        if (entitiyMovement != null) { entitiyMovement.enabled = false; }



    }

    private IEnumerator Animate() // animate change in position - jump then move below screen
    {
        float elapsed = 0f;
        float duration = 3f;

        float jumpVelocity = 10f;
        float gravity = -36f; 

        Vector3 velocity = Vector3.up * jumpVelocity;

        while (elapsed < duration)
        {
            transform.position += velocity * Time.deltaTime;
            velocity.y += gravity * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
