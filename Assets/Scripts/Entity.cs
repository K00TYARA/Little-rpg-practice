using System;
using UnityEngine;

public class Entity : MonoBehaviour {

    protected Rigidbody2D rb;
    protected Animator animator;

    [Header("Attack details")]
    [SerializeField] protected float attackRadius;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected LayerMask whatIsTarget;

    [Header("Movement details")]
    [SerializeField] protected float moveSpeed = 4;
    [SerializeField] protected float jumpForce = 8;
    protected int facingDir = 1;
    private float xInput;
    private bool facingRight = true;
    protected bool canMove = true;
    private bool canJump = true;

    [Header("Collision check")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Update() {
        HandleCollision();

        HandleInput();
        HandleMovement();
        HandleFlip();

        HandleAnimation();
    }

    public void DamageTargets() {
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsTarget);

        foreach (Collider2D enemy in enemyColliders) {
            Entity entityTarget = enemy.GetComponent<Entity>();
            entityTarget.TakeDamage();
        }
    }

    private void TakeDamage() {
        //throw new NotImplementedException();
    }

    public void EnableMovementAndJump(bool enable) {
        canMove = enable;
        canJump = enable;
    }

    protected virtual void HandleAnimation() {
        animator.SetFloat("yVelocity", rb.linearVelocityY);
        animator.SetFloat("xVelocity", rb.linearVelocityX);


        animator.SetBool("isGrounded", isGrounded);
    }

    protected virtual void HandleMovement() {
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
            HandleAttack();
        }
    }

    private void TryToJump() {
        if (isGrounded && canJump)
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
    }

    protected virtual void HandleAttack() {
        if (isGrounded) {
            animator.SetTrigger("attack");
        }
    }
    protected void HandleFlip() {
        if (rb.linearVelocityX < 0 && facingRight == true) {
            Flip();
        } else if (rb.linearVelocityX > 0 && facingRight == false) {
            Flip();
        }
    }

    private void Flip() {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDir *= -1;
    }

    protected virtual void HandleCollision() {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}