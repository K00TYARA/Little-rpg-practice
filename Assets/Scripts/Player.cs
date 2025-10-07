using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class Player : Entity {

    private float xInput;

    [Header("Movement details")]
    [SerializeField] protected float moveSpeed = 4;
    [SerializeField] protected float jumpForce = 8;
    [Space]
    [SerializeField] private float dashingPower = 12f;
    [SerializeField] private float dashingTime = .4f;
    [SerializeField] private float dashingCooldown = 10f;
    
    private float dashCooldownRemaining;
    private bool dashReady = true;

    protected override void Update() {
        if (state == EntityState.Die || state == EntityState.Hit) return;
        base.Update();

        HandleInput();
        HandleDashSkillVisual();
    }

    private void HandleDashSkillVisual() {
        if (dashCooldownRemaining > 0) {
            UI.instance.DashSkillTimer.text = (dashCooldownRemaining -= Time.deltaTime).ToString("F1");
        } else {
            UI.instance.DashSkillTimer.gameObject.SetActive(false);
            UI.instance.DashSkillImage.color = Color.white;
        }
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
        UI.instance.DashSkillTimer.gameObject.SetActive(true);
        UI.instance.DashSkillImage.color = new Color(166f/255f, 166f/255f, 166f/255f);
        dashCooldownRemaining = dashingCooldown;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        dashReady = false;

        rb.linearVelocity = new Vector2(facingDir * dashingPower, 0);
        yield return new WaitForSeconds(dashingTime);

        rb.linearVelocity = new Vector2(0, 0);
        rb.gravityScale = originalGravity;
        SetState(EntityState.Attack);
        yield return new WaitForSeconds(dashingCooldown);

        dashReady = true;
    }

    protected override void HandleAnimation() {
        base.HandleAnimation();
        animator.SetFloat("yVelocity", rb.linearVelocityY);
        animator.SetBool("isGrounded", isGrounded);
    }

    protected override void TakeDamage() {
        base.TakeDamage();
        UI.instance.HealthBar.fillAmount = currentHealth / maxHealth;
    }

    public override void EntityDie() {
        animator.enabled = false;
    }
}
