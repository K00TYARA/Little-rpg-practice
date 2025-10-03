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
    [SerializeField] protected float maxHealth = 1;
    [SerializeField] protected float currentHealth;

    [Header("Attack details")]
    [SerializeField] protected float attackRadius;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected LayerMask whatIsTarget;
    protected float currGravityScale;

    // Die or Hit
    private float fadeDuration = 3f;
    protected bool isDie = false;
    protected bool isHit = false;

    [Header("Collision check")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    protected bool isGrounded;

    protected int facingDir = 1;
    protected bool facingRight = true;
    protected bool canMove = true;
    protected bool canAttack = true;

    public bool isGameOver = false;

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
        currGravityScale = rb.gravityScale;
    }

    protected virtual void Update() {
        if (isDie || isHit) return;
        HandleCollision();
        HandleMovement();
        HandleAnimation();
        HandleFlip();
    }

    protected virtual void HandleMovement() { }

    protected virtual void HandleAnimation() {
        animator.SetFloat("yVelocity", rb.linearVelocityY);
        animator.SetFloat("xVelocity", rb.linearVelocityX);


        animator.SetBool("isGrounded", isGrounded);
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

    protected virtual void TakeDamage() {
        currentHealth -= 1;
        isHit = true;

        if (currentHealth <= 0)
            Die();
        else
            Hit();
    }

    protected virtual void Die() {
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
        Bounced();
    }

    private void Bounced() {
        rb.gravityScale = 8;

        float knockBackX = -facingDir * 3f;
        float knockBackY = 15f;

        rb.linearVelocity = new Vector2(knockBackX, knockBackY);
    }

    protected virtual void HandleAttack() {
        if (isGrounded) {
            animator.SetTrigger("attack");
        }
    }

    public void DamageTargets() {

        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsTarget);

        if (enemyColliders.Length != 0) {
            Entity entityTarget = enemyColliders[0].GetComponent<Entity>();
            if (entityTarget.transform.position.x > transform.position.x && entityTarget.facingRight == true ||
                entityTarget.transform.position.x < transform.position.x && entityTarget.facingRight == false) {
                entityTarget.Flip();
            }
            entityTarget.TakeDamage();
        }
    }

    public virtual void EntityDie() {
        animator.enabled = false;
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

    public void PlayerDie() {
        isGameOver = true;
        UI.instance.EnablegameOverUI();
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

    public virtual void EnableMovement(bool enable) {
        if (!canAttack) return;
        canMove = enable;

        if (enable == true) 
            isHit = false;
    }

    protected virtual void HandleCollision() {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}