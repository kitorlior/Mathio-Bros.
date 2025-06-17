using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Camera cam;
    private Collider2D coll;

    private float inputAxis;
    private Vector2 velocity;

    public float moveSpeed = 8f;
    public float maxJumpHeight = 5f;
    public float maxJumpTime = 1f;
    public PhotonView view;

    //public bool playerchose = false;

    public float JumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    public float Gravity => (-2f * maxJumpHeight) / Mathf.Pow((maxJumpTime / 2f), 2);

    public bool Grounded { get; private set; }
    public bool Jumping { get; private set; }
    public bool Running => Mathf.Abs(velocity.x) > 0.25f || Mathf.Abs(inputAxis) > 0.25f;
    public bool Sliding => (inputAxis > 0f && velocity.x < 0f) || (inputAxis < 0f && velocity.x > 0f);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        view = GetComponent<PhotonView>();
        cam = Camera.main;
    }

    private void OnDisable()
    {
        // disable for animations
        rb.bodyType = RigidbodyType2D.Kinematic;
        coll.enabled = false;
        velocity = Vector2.zero;
        Jumping = false;
    }

    private void OnEnable()
    {
        //return to normal after animations
        rb.bodyType = RigidbodyType2D.Dynamic;
        coll.enabled = true;
        velocity = Vector2.zero;
        Jumping = false;
    }

    private void Update()
    {
        if (GameManager.Instance.isMulti && !view.IsMine) { return; }
        HorizontalMovement();
        Grounded = rb.Raycast(Vector2.down);
        if (Grounded)
        {
            GroundedMovement();
        }
        ApplyGravity();
    }

    private void HorizontalMovement() // handle mario movement
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

    private void GroundedMovement() // handle jumping
    {
        velocity.y = Mathf.Max(velocity.y, 0f);
        Jumping = velocity.y > 0f;

        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = JumpForce;
            Jumping = true;
        }
    }

    private void ApplyGravity() // handle gravity
    {
        bool falling = velocity.y < 0f || !Input.GetButton("Jump");
        float multiplier = falling ? 2f : 1f;

        velocity.y += Gravity * Time.deltaTime * multiplier;

        velocity.y = Mathf.Max(velocity.y, Gravity / 2f); //hitting terminal velocity
    }

    private void FixedUpdate()
    {
        Vector2 position = rb.position;
        position += velocity * Time.fixedDeltaTime;

        //keep player within screen
        Vector2 leftEdge = cam.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = cam.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);

        rb.MovePosition(position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ( collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (transform.DotTest(collision.transform, Vector2.down)) // check if landing on top of enemy
            {
                velocity.y = JumpForce / 2f; // bounce off enemy
                Jumping = true;
            }
        }
        if (collision.gameObject.layer != LayerMask.NameToLayer("PowerUp"))// handle bumping head when jumping
        {
            if (transform.DotTest(collision.transform, Vector2.up))
            {
                velocity.y = 0f;
            }
        }
    }
}
