using UnityEngine;

public class Player : MonoBehaviour {

    // Components
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Attack details")]
    [SerializeField] private float attackRadius;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask whatIsEnemy;

    [Header("Movement parametres")]
    private float xInput;
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float jumpForce = 8;
    private bool facingRight = true;
    private bool canMove = true;
    private bool canJump = true;

    [Header("Collision check")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update() {
        HandleCollision();

        HandleInput();
        HandleMovement();
        HandleFlip();

        HandleAnimation();
    }

    public void DamageEnemies() {
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsEnemy);

        foreach (Collider2D enemy in enemyColliders) {
            enemy.GetComponent<Enemy>().TakeDamage();
        }
    }

    public void EnableMovementAndJump(bool enable) {
        canMove = enable;
        canJump = enable;
    }

    private void HandleAnimation() {
        animator.SetFloat("linearY", rb.linearVelocityY);
        animator.SetFloat("linearX", rb.linearVelocityX);


        animator.SetBool("isGrounded", isGrounded);
    }

    private void HandleMovement() {
        if (canMove)
            rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocityY);
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
    }

    private void HandleInput() {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space)) {
            TryToJump();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            TryToAttack();
        }
    }

    private void TryToJump() {
        if (isGrounded && canJump)
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
    }

    private void TryToAttack() {
        if (isGrounded) { 
            animator.SetTrigger("attack");
        }

    }

    private void HandleFlip() {
        if (rb.linearVelocityX < 0 && facingRight == true) {
            Flip();
        } else if (rb.linearVelocityX > 0 && facingRight == false) {
            Flip();
        }
    }

    private void Flip() {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    private void HandleCollision() {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}