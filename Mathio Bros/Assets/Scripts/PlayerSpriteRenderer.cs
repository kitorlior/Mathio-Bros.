using UnityEngine;

public class PlayerSpriteRenderer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private PlayerMovement movement;

    public Sprite Idle;
    public Sprite Jump;
    public Sprite Slide;
    public AnimatedSprite Run;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        movement = GetComponentInParent<PlayerMovement>();
    }

    private void OnEnable() { spriteRenderer.enabled = true; }

    private void OnDisable()
    {
        spriteRenderer.enabled = false;
        Run.enabled = false;
    }

    private void LateUpdate()
    {
        Run.enabled = movement.Running;

        if (movement.Jumping) { spriteRenderer.sprite = Jump; }
        else if (movement.Sliding) { spriteRenderer.sprite = Slide; }
        else if (!movement.Running) { spriteRenderer.sprite = Idle; }
    }
}
