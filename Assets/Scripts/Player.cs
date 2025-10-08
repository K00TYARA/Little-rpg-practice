using System.Collections;
using UnityEngine;


public class Player : Entity {

    private float xInput;
    public static Player instance;

    [Header("Movement details")]
    [SerializeField] protected float moveSpeed = 4;
    [SerializeField] protected float jumpForce = 8;
    [Space]
    [SerializeField] private float dashPower = 12f;
    [SerializeField] private float dashTime = .4f;
    [SerializeField] private float dashCooldown = 10f;

    private bool dashReady = true;

    protected override void Awake() {
        base.Awake();
        instance = this;
    }

    protected override void Update() {
        if (state == EntityState.Die || state == EntityState.Hit) return;
        base.Update();

        HandleInput();
    }

    private void HandleInput() {

        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space)) {
            HandleJump();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            HandleAttack();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && IsActionAndMovementAllowed() && dashReady) {
            animator.SetTrigger("dash");
            SetState(EntityState.Dash);
            StartCoroutine(HandleDash());
        }
    }

    protected override void Move() {
        if (IsActionAndMovementAllowed()) {
            rb.linearVelocity = new Vector2(xInput != 0 ? xInput * moveSpeed : 0, rb.linearVelocityY);
            SetState(EntityState.Move);
        }
    }

    private void HandleJump() {
        if (isGrounded && IsActionAndMovementAllowed()) {
            rb.gravityScale = 2.5f;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            SetState(EntityState.Jump);
        }
    }

    private IEnumerator HandleDash() {
        UI.instance.ShowDashCooldown(dashCooldown);

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        dashReady = false;

        rb.linearVelocity = new Vector2(facingDir * dashPower, 0);
        yield return new WaitForSeconds(dashTime);

        rb.linearVelocity = new Vector2(0, 0);
        rb.gravityScale = originalGravity;
        yield return new WaitForSeconds(dashCooldown - dashTime);

        dashReady = true;
    }

    protected override void HandleAnimation() {
        base.HandleAnimation();
        animator.SetFloat("yVelocity", rb.linearVelocityY);
        animator.SetBool("isGrounded", isGrounded);
    }

    protected override void TakeDamage() {
        base.TakeDamage();
        UI.instance.HandleHealthBarVisual(currentHealth, maxHealth);
    }

    public override void EntityDie() {
        animator.enabled = false;
    }
}
