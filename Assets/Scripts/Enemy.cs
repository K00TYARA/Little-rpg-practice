using System.Collections;
using UnityEngine;

public class Enemy : Entity {

    [Header("Movement details")]
    [SerializeField] protected float moveSpeed = 4;

    private float enemyFadeDuration = 3f;

    private bool PlayerDetected;

    protected override void Update() {
        HandleAnimation();
        if (isGameOver) {
            rb.linearVelocityX = 0;
            return;
        }

        if (state == EntityState.Die || state == EntityState.Hit) return;
        base.Update();
        HandleAttack();
    }

    protected override void HandleMove() {
        if (IsActionAndMovementAllowed()) {
            rb.linearVelocity = new Vector2(facingDir * moveSpeed, rb.linearVelocityY);
            SetState(EntityState.Move);
        }
    }

    protected override void HandleAttack() {
        if (PlayerDetected) {
            base.HandleAttack();
        }
    }

    protected override void Die() {
        base.Die();
        UI.instance.AddKillCount();
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

    protected override void HandleFlip() {
        float playerPositionX = Player.instance.transform.position.x;
        if (playerPositionX < transform.position.x && facingRight == true && IsActionAndMovementAllowed() ||
            playerPositionX > transform.position.x && facingRight == false && IsActionAndMovementAllowed()) {
            Flip();
        }
    }

    protected override void HandleCollision() {
        base.HandleCollision();
        PlayerDetected = Physics2D.OverlapCircle(attackPoint.position, attackRadius, whatIsTarget);
    }
}
