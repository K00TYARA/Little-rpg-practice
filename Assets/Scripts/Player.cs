using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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
    }

    protected override void HandleAnimation() {
        base.HandleAnimation();
        animator.SetFloat("yVelocity", rb.linearVelocityY);
        animator.SetBool("isGrounded", isGrounded);
    }

    public void InputMove(InputAction.CallbackContext context) {
        xInput = context.ReadValue<Vector2>().x;
    }

    public void InputJump() {
        HandleJump();
    }

    public void InputAttack() {
        HandleAttack();
    }

    public void InputDash() {
        StartCoroutine(Dash());
    }

    protected override void HandleMove() {
        if (IsActionAndMovementAllowed()) {
            if (xInput == 0) {
                SetState(EntityState.Idle);
            } else {
                SetState(EntityState.Move);
            }
            rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocityY);
        }
    }

    private void HandleJump() {
        if (isGrounded && IsActionAndMovementAllowed()) {
            rb.gravityScale = 2.5f;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            SetState(EntityState.Jump);
        }
    }

    private IEnumerator Dash() {
        if (IsActionAndMovementAllowed() && dashReady) {
            animator.SetTrigger("dash");
            SetState(EntityState.Dash);
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
    }

    protected override void TakeDamage() {
        base.TakeDamage();
        UI.instance.HandleHealthBarVisual(currentHealth, maxHealth);
    }

    protected override void Die() {
        base.Die();
        isGameOver = true;
        UI.instance.EnableGameOverUI();
    }
}
