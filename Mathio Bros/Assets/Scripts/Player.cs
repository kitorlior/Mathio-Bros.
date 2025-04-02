using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerSpriteRenderer smallRenderer;
    public PlayerSpriteRenderer bigRenderer;

    private DeathAnimation deathAnimation;

    public bool big => bigRenderer.enabled;
    public bool small => smallRenderer.enabled;
    public bool dead => deathAnimation.enabled;

    private void Awake()
    {
        deathAnimation = GetComponent<DeathAnimation>();
    }

    public void Hit()
    {
        if (big) { Shrink(); }
        else { Death(); }
    }

    private void Death()
    {
        smallRenderer.enabled = false;
        bigRenderer.enabled = false;
        deathAnimation.enabled = true;

        GameManager.Instance.ResetLevel(3f);
        
    }

    private void Shrink()
    {
        throw new NotImplementedException();
    }
}
