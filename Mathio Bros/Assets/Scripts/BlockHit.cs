using System;
using System.Collections;
using UnityEngine;

public class BlockHit : MonoBehaviour
{
    public int maxHits = -1; // change in editor to positive if not infinate
    public Sprite emptyBlock;
    public GameObject item;
    public bool isNum = false;
    public int blockNumber = 0;
    public Sprite[] numberBlocks;

    private bool animating;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!animating && maxHits != 0 && collision.gameObject.CompareTag("Player"))
        {
            if (collision.transform.DotTest(transform, Vector2.up))
            {
                if (isNum) { HitNumbered(); }
                else { Hit(); } // only hit block when resting and coming from below (and only player)
            }
        }
    }

    private void Hit()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
        maxHits--; // update num of remaining hits

        if (maxHits == 0) //update to empty block after last hit
        {
            spriteRenderer.sprite = emptyBlock;
        }

        if (item != null) // if block has item spawn it after hit
        {
            Instantiate(item, transform.position, Quaternion.identity);
        }
             
        StartCoroutine(Animate()); // animate block
    }

    private void HitNumbered()
    {

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = numberBlocks[blockNumber];//change to correct number
        
        EquationLogic equationLogic = GameObject.Find("Equation Visualizer").GetComponent<EquationLogic>();
        equationLogic.CheckHit(blockNumber);
        
        StartCoroutine(Animate()); // animate block
    }

    private IEnumerator Animate() // moves block .5 units up and back down
    {
        animating = true;

        Vector3 restingPosition = transform.localPosition;
        Vector3 animatedPosition = restingPosition + Vector3.up * 0.5f;

        yield return Move(restingPosition, animatedPosition); // move up
        yield return Move(animatedPosition, restingPosition); // move back down

        animating = false;
    }

    private IEnumerator Move(Vector3 from, Vector3 to) // animate change in position
    {
        float elapsed = 0f;
        float duration = 0.125f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localPosition = Vector3.Lerp(from, to, t); // change position based on progress
            elapsed += Time.deltaTime;

            yield return null;
        }
        //loop might finish before reaching, update manually
        transform.localPosition = to;
    }
}
