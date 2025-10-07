using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Entity;

public class Entity : MonoBehaviour {

    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator animator;

    [Header("Health")]
    [SerializeField] protected float maxHealth = 1;
    [SerializeField] protected float currentHealth;

    [Header("Attack details")]
    [SerializeField] protected float attackRadius;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected LayerMask whatIsTarget;

    [Header("Collision check")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    protected bool isGrounded;

    protected int facingDir = 1;
    protected bool facingRight = true;

    private float enemyFadeDuration = 3f;
    [HideInInspector] public static bool isGameOver = false;

    public enum EntityState {
        Idle, 
        Move,
        Jump, 
        Fall, 
        Attack,
        Hit,
        Dash,
        Die
    }

    protected EntityState state = EntityState.Idle;

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
    }

    protected virtual void Update() {
        if (isGameOver || state == EntityState.Die || state == EntityState.Hit) return;

        if (IsActionAndMovementAllowed()) {
            HandleFlip();
            Move();
        }

        HandleCollision();
        HandleAnimation();
    }

    protected void SetState(EntityState newState) {
        if (state != newState) {
            state = newState;
        }
    }

    protected virtual void Move() {}

    protected virtual void HandleAnimation() {
        animator.SetFloat("xVelocity", rb.linearVelocityX);
    }

    protected virtual void HandleFlip() {
        if (rb.linearVelocityX < 0 && facingRight ||
            rb.linearVelocityX > 0 && !facingRight) {
            Flip();
        }
    }

    protected void Flip() {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDir *= -1;
    }

    protected virtual void Die() {
        animator.SetTrigger("die");
        Bounced();
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
        if (isGrounded && IsActionAndMovementAllowed()) {
            animator.SetTrigger("attack");
            rb.linearVelocityX = 0;
            SetState(EntityState.Attack);
        }
    }
    
    public void DamageTargets() {
        Collider2D entityCollider = Physics2D.OverlapCircle(attackPoint.position, attackRadius, whatIsTarget);

        if (entityCollider != null) {
            Entity entityTarget = entityCollider.GetComponent<Entity>();

            float attackerPosition = gameObject.transform.position.x;
            float entityTargetPosition = entityTarget.transform.position.x;

            if (entityTargetPosition < attackerPosition && !entityTarget.facingRight ||
                entityTargetPosition > attackerPosition && entityTarget.facingRight) {
                entityTarget.Flip();
            }

            entityTarget.TakeDamage();
        }
    }

    protected virtual void TakeDamage() {
        currentHealth -= 1;
        if (currentHealth <= 0) {
            Die();
            SetState(EntityState.Die);
        } else {
            Hit();
            SetState(EntityState.Hit);
        }
    }

    public virtual void EntityDie() {
        animator.enabled = false;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut() {
        Color startColor = sr.color;
        float elapsed = 0f;

        while (elapsed < enemyFadeDuration) {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / enemyFadeDuration);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }

    public void PlayerDie() {
        isGameOver = true;
        UI.instance.EnableGameOverUI();
    }

    protected virtual void HandleCollision() {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public void UpdateState() {
        if (isGrounded && Mathf.Abs(rb.linearVelocityX) > 0f)
            SetState(EntityState.Move);
        else if (isGrounded)
            SetState(EntityState.Idle);
        else if (rb.linearVelocityY > 0f)
            SetState(EntityState.Jump);
        else if (rb.linearVelocityY < -0f)
            SetState(EntityState.Fall);
    }

    protected bool IsActionAndMovementAllowed() {
        return state != EntityState.Attack && state != EntityState.Dash &&
               state != EntityState.Die && state != EntityState.Hit;
    }
}