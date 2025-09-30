using System.Collections;
using System.Linq;
using UnityEngine;

public class Entity : MonoBehaviour {

    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator animator;
    protected Collider2D col;
    protected bool isPlayer;

    [Header("Health")]
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private int currentHealth;

    [Header("Attack details")]
    [SerializeField] protected float attackRadius;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected LayerMask whatIsTarget;
    protected float currGravityScale;

    // Die or Hit
    private float fadeDuration = 1f;
    protected bool isDie = false;
    protected bool isHit = false;

    [Header("Movement details")]
    [SerializeField] protected float moveSpeed = 4;
    [SerializeField] protected float jumpForce = 8;
    protected int facingDir = 1;
    private float xInput;
    protected bool facingRight = true;
    protected bool canMove = true;
    private bool canJump = true;
    protected bool canAttack = true;

    [Header("Collision check")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    protected bool isGrounded;

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
        isPlayer = true;
        currGravityScale = rb.gravityScale;
    }

    protected virtual void Update() {
        if (isDie || isHit) return;
        HandleCollision();

        HandleInput();
        HandleMovement();
        HandleFlip();

        HandleAnimation();
    }

    public void DamageTargets() {
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsTarget);

        //foreach (Collider2D enemy in enemyColliders) {
        //    Entity entityTarget = enemy.GetComponent<Entity>();
        //    entityTarget.TakeDamage();
        //}

        // Attack only one enemy, even if Colliders more than one
        if (enemyColliders.Length != 0) {
            Entity entityTarget = enemyColliders[0].GetComponent<Entity>();
            if (entityTarget.transform.position.x > transform.position.x && entityTarget.facingRight == true ||
                entityTarget.transform.position.x < transform.position.x && entityTarget.facingRight == false) {
                entityTarget.Flip();
            }
            entityTarget.TakeDamage();
        }
    }

    private void TakeDamage() {
        currentHealth -= 1;
        isHit = true;

        if (currentHealth <= 0)
            Die();
        else
            Hit();
    }

    protected void Die() {
        animator.SetTrigger("die");
        canMove = false;
        canAttack = false;

        if (!isDie) {
            isDie = true;
            Bounced();
        }
    }

    protected void Hit() {
        animator.SetTrigger("hit");
        canMove = false;

        Bounced();
    }

    private void Bounced() {
        rb.gravityScale = 8;

        float knockBackX = -facingDir * 3f;
        float knockBackY = 15f;

        rb.linearVelocity = new Vector2(knockBackX, knockBackY);
    }

    public void DisableAnimationAndDestroyEntity() {
        animator.enabled = false;
        if (!isPlayer)
            StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut() {
        Color startColor = sr.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration) {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }

    public void PlayerDieStopMove() {
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        Collider2D[] all = Object.FindObjectsByType<Collider2D>(FindObjectsSortMode.None);

        Collider2D[] enemies = all
            .Where(en => en.gameObject.layer == enemyLayer)
            .ToArray();

        foreach (Collider2D enemy in enemies) {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.canAttack = false;
            enemyScript.canMove = false;
        }
    }

    public void EnableMovementAndJump(bool enable) {
        if (!canAttack) return;
        canMove = enable;
        canJump = enable;
        isHit = false;
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
        if (isGrounded && canJump) {
            rb.gravityScale = 2.5f;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        }
    }

    protected virtual void HandleAttack() {
        if (isGrounded) {
            animator.SetTrigger("attack");
        }
    }

    protected virtual void HandleFlip() {
        if (rb.linearVelocityX < 0 && facingRight == true ||
            rb.linearVelocityX > 0 && facingRight == false) {
            Flip();
        }
    }

    protected void Flip() {
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