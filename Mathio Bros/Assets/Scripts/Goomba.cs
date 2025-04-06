using UnityEngine;

public class Goomba : MonoBehaviour
{
    public Sprite flatSprite;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if (player.starpower) { Hit(); } // when mario starpower, mario hits goomba
            // flatten when mario lands on goomba
            else if (collision.transform.DotTest(transform, Vector2.down)) { Flatten(); }
            else { player.Hit(); } // otherwise hit player
        }
    }

    private void Flatten() // handle flattning goomba
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<EntitiyMovement>().enabled = false;
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<SpriteRenderer>().sprite = flatSprite;
        Destroy(gameObject, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Shell"))
        {
            // get hit by koopa shell
            Hit();
        }
    }

    private void Hit() // simulate death
    {
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<DeathAnimation>().enabled = true;
        Destroy(gameObject, 3f);
    }
}
