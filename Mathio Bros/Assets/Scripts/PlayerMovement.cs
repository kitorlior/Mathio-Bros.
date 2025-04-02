using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Camera cam;

    private float inputAxis;
    private Vector2 velocity;

    public float moveSpeed = 8f;
    public float maxJumpHeight = 5f;
    public float maxJumpTime = 1f;

    public float jumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    public float gravity => (-2f * maxJumpHeight) / Mathf.Pow((maxJumpTime / 2f), 2);

    public bool Grounded { get; private set; }
    public bool Jumping { get; private set; }
    public bool Running => Mathf.Abs(velocity.x) > 0.25f || Mathf.Abs(inputAxis) > 0.25f;
    public bool Sliding => (inputAxis > 0f && velocity.x < 0f) || (inputAxis < 0f && velocity.x > 0f);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    private void Update()
    {
        HorizontalMovement();
        Grounded = rb.Raycast(Vector2.down);
        if (Grounded)
        {
            GroundedMovement();
        }
        ApplyGravity();
    }

    private void HorizontalMovement()
    {
        inputAxis = Input.GetAxis("Horizontal");
        velocity.x = Mathf.MoveTowards(velocity.x, inputAxis * moveSpeed, moveSpeed * Time.deltaTime);

        if (rb.Raycast(Vector2.right * velocity.x))
        {
            velocity.x = 0;
        }

        if (velocity.x > 0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (velocity.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void GroundedMovement()
    {
        velocity.y = Mathf.Max(velocity.y, 0f);
        Jumping = velocity.y > 0f;

        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = jumpForce;
            Jumping = true;
        }
    }

    private void ApplyGravity()
    {
        bool falling = velocity.y < 0f || !Input.GetButton("Jump");
        float multiplier = falling ? 2f : 1f;

        velocity.y += gravity * Time.deltaTime * multiplier;

        velocity.y = Mathf.Max(velocity.y, gravity / 2f); //hitting terminal velocity
    }

    private void FixedUpdate()
    {
        Vector2 position = rb.position;
        position += velocity * Time.fixedDeltaTime;

        Vector2 leftEdge = cam.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);

        rb.MovePosition(position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ( collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (transform.DotTest(collision.transform, Vector2.down))
            {
                velocity.y = jumpForce / 2f;
                Jumping = true;
            }
        }
        if (collision.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
        {
            if (transform.DotTest(collision.transform, Vector2.up))
            {
                velocity.y = 0f;
            }
        }
    }
}
