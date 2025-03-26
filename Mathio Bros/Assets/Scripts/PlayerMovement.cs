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
    public float gravity => (-2f * maxJumpHeight) / Mathf.Pow((maxJumpTime/2f),2);

    public bool grounded {  get; private set; }
    public bool jumping { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    private void Update()
    {
        HorizontalMovement();

        //rb.Raycast();
    }

    private void HorizontalMovement()
    {
        inputAxis = Input.GetAxis("Horizontal");
        velocity.x = Mathf.MoveTowards(velocity.x, inputAxis*moveSpeed, moveSpeed*Time.deltaTime);
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
}
