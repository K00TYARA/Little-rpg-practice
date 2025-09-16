using System;
using UnityEngine;

public class Player : MonoBehaviour {

    // Components
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Animator animator;

    [Header("Movement parametres")]
    private float xInput;
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float jumpForce = 8;
    private bool facingRight = true;
    
    [Header("Collision check")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;
    
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update() {
        HandleCollision();

        HandleInput();
        HandleMovement();
        HandleFlip();

        HandleAnimation();
    }

    private void HandleAnimation() {
        bool isRunning = rb.linearVelocityX != 0;
        animator.SetBool("isRunning", isRunning);
        animator.SetFloat("linearY", rb.linearVelocityY);
        animator.SetFloat("linearX", xInput);


        animator.SetBool("isGrounded", isGrounded);
    }

    private void HandleMovement() {
        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocityY);
    }

    private void HandleInput() {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space)) {
            Jump();
        }
    }

    private void Jump() {
        if (isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
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
    }
}