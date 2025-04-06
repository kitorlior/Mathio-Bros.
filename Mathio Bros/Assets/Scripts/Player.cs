using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerSpriteRenderer smallRenderer;
    public PlayerSpriteRenderer bigRenderer;

    private DeathAnimation deathAnimation;
    private PlayerSpriteRenderer activeRenderer;
    private CapsuleCollider2D capsuleCollider;

    public bool big => bigRenderer.enabled;
    public bool small => smallRenderer.enabled;
    public bool dead => deathAnimation.enabled;

    public bool starpower {  get; private set; }

    private void Awake()
    {
        deathAnimation = GetComponent<DeathAnimation>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        activeRenderer = smallRenderer;
    }

    public void Hit()
    {
        if (!dead &&!starpower) // only get hit in normal state
        {
            if (big) { Shrink(); } // if big only shrink not die
            else { Death(); }
        }
    }

    private void Death() // handle death animation
    {
        smallRenderer.enabled = false;
        bigRenderer.enabled = false;
        deathAnimation.enabled = true;

        GameManager.Instance.ResetLevel(3f);
        
    }

    public void Grow() // handle change to big mario
    {
        smallRenderer.enabled = false;
        bigRenderer.enabled = true;
        activeRenderer = bigRenderer;

        //update collider to match sprite
        capsuleCollider.size = new Vector2(1f, 2f);
        capsuleCollider.offset = new Vector2(0, 0.5f);

        StartCoroutine(ScaleAnimation()); //start change animation
    }

    private void Shrink() // handle change back to small mario
    {
        smallRenderer.enabled = true;
        bigRenderer.enabled = false;
        activeRenderer = smallRenderer;

        //update collider to match sprited
        capsuleCollider.size = new Vector2(1f, 1f);
        capsuleCollider.offset = new Vector2(0, 0f);

        StartCoroutine(ScaleAnimation()); //start change animation
    }

    private IEnumerator ScaleAnimation() // handle animation on change in mario size
    {
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (Time.frameCount % 4 == 0) // switch sprite every 4 frames
            {
                smallRenderer.enabled = !smallRenderer.enabled;
                bigRenderer.enabled = !smallRenderer.enabled;
            }

            yield return null;
        }

        // make sure animation ends on correct sprite
        smallRenderer.enabled = false;
        bigRenderer.enabled = false;
        activeRenderer.enabled = true;
    }

    public void Starpower(float duration = 10f)
    {
        StartCoroutine(StarpowerAnimation(duration)); // animate
    }

    private IEnumerator StarpowerAnimation(float duration)
    {
        starpower = true;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (Time.frameCount % 4 == 0) // randomize sprite color every 4 frames
            {
                activeRenderer.spriteRenderer.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            }
            yield return null;
        }
        
        //return to normal sprite
        activeRenderer.spriteRenderer.color = Color.white;
        starpower = false;
    }
}
