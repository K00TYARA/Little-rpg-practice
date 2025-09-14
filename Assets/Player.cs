using UnityEngine;

public class Player : MonoBehaviour {

    // Components
    private Rigidbody2D rb;

    // Movement
    private float xInput;
    [SerializeField] private float moveSpeed = 2;
    [SerializeField] private float jumpForce = 8;

    [Header("Collision check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {

        CollisionChecks();
        HandleInput();

        HandleMovement();
    }

    private void HandleMovement() {
        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocityY);
    }

    private void HandleInput() {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.Space) && isGrounded) {
            Jump();
        }
    }

    private void Jump() {
        rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
    }

    private void CollisionChecks() {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}