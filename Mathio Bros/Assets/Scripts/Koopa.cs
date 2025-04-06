using System;
using UnityEngine;

public class Koopa : MonoBehaviour
{
    public Sprite shellSprite;
    public float shellSpeed;

    private bool shelled;
    private bool pushed;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!shelled && collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player.starpower) { Hit(); } // when starpower koopa gets hit - not mario
            else if (collision.transform.DotTest(transform, Vector2.down)) { EnterShell(); } // when hit go into shell
            else { player.Hit(); } // hit player
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (shelled && collision.gameObject.CompareTag("Player")) // only when hitting player while in shell
        {
            if (!pushed) // if not already pushed by player - player push shell
            {
                Vector2 direction = new Vector2(transform.position.x - collision.transform.position.x, 0f);
                PushShell(direction);
            }
            else
            {
                Player player = collision.GetComponent<Player>();

                if (player.starpower) { Hit(); } // when starpower mario can hit shell
                else { player.Hit(); } // otherwise player hit by shell
            }
        }
        else if (!shelled && collision.gameObject.layer == LayerMask.NameToLayer("Shell"))
        {
            // when koopa hit by another koopa shell
            Hit();
        }
    }

    private void Hit() // simulate death
    {
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<DeathAnimation>().enabled = true;
        Destroy(gameObject, 3f);
    }

    private void PushShell(Vector2 direction) // handle shell push
    {
        pushed = true;

        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        EntitiyMovement movement = GetComponent<EntitiyMovement>();
        movement.direction = direction.normalized;
        movement.speed = shellSpeed;
        movement.enabled = true;

        gameObject.layer = LayerMask.NameToLayer("Shell"); 
    }

    private void EnterShell() // handle entering shell
    {
        GetComponent<EntitiyMovement>().enabled = false;
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<SpriteRenderer>().sprite = shellSprite;
    }

    private void OnBecameInvisible() // destroy shell when exiting screen
    {
        if (pushed)
        {
            Destroy(gameObject);
        }
    }
}
