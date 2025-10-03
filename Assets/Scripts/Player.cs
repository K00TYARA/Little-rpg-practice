//using System;
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
    
    private bool canDash = true;
    private bool isDashing = false;
    private float coolDown;

    protected bool canJump = true;

    protected override void Awake() {
        base.Awake();
        isPlayer = true;
    }

    protected override void Update() {
        if (isDie || isHit) return;
        base.Update();
        HandleInput();

        if (coolDown > 0) {
            UI.instance.DashSkillTimer.text = (coolDown -= Time.deltaTime).ToString("F1");
        } else {
            UI.instance.DashSkillTimer.gameObject.SetActive(false);
            UI.instance.DashSkillImage.color = Color.white;
            canDash = true;
        }
    }

    private void HandleInput() {
        if (!isDashing)
            xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space)) {
            HandleJump();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            HandleAttack();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) {
            animator.SetTrigger("dash");
            StartCoroutine(HandleDash());
        }
    }

    protected override void HandleMovement() {
        if (canMove)
            rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocityY);
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
    }

    private void HandleJump() {
        if (isGrounded && canJump) {
            rb.gravityScale = 2.5f;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        }
    }

    private IEnumerator HandleDash() {
        UI.instance.DashSkillTimer.gameObject.SetActive(true);
        UI.instance.DashSkillImage.color = new Color(166f/255f, 166f/255f, 166f/255f);
        coolDown = dashingCooldown;
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        xInput = facingDir;
        rb.linearVelocity = new Vector2(facingDir * dashingPower, 0f);
        yield return new WaitForSeconds(dashingTime);

        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
    }

    protected override void TakeDamage() {
        base.TakeDamage();
        UI.instance.HealthBar.fillAmount = currentHealth / maxHealth;
    }

    public override void EnableMovement(bool enable) {
        if (!canAttack) return;
        base.EnableMovement(enable);
        canJump = enable;
    }

    public override void EntityDie() {
        animator.enabled = false;
    }
}
