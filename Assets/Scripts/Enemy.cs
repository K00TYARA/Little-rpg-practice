using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class Enemy : Entity {

    [Header("Movement details")]
    [SerializeField] protected float moveSpeed = 4;

    private bool PlayerDetected;
    private bool isAttacking = false;

    protected override void Awake() {
        base.Awake();
        isPlayer = false;
    }

    protected override void Update() {
        if (isDie || isHit) return;
        base.Update();
        HandleAttack();
    }

    protected override void HandleMovement() {
        if (canMove) {
            rb.linearVelocity = new Vector2(facingDir * moveSpeed, rb.linearVelocityY);
        } else {
            rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
        }
    }

    protected override void HandleAnimation() {
        animator.SetFloat("xVelocity", rb.linearVelocityX);
    }

    protected override void HandleAttack() {
        if (PlayerDetected && canAttack && !isAttacking) {
            animator.SetTrigger("attack");
            isAttacking = true;
        }
    }

    protected override void HandleFlip() {
        int playerLayer = LayerMask.NameToLayer("Player");

        Transform[] all = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);

        Transform[] players = all
            .Where(en => en.gameObject.layer == playerLayer)
            .ToArray();

        Transform player = players[0];

        if (player.position.x < transform.position.x && facingRight == true && !isAttacking ||
            player.position.x > transform.position.x && facingRight == false && !isAttacking) {
            Flip();
        }
    }

    protected override void HandleCollision() {
        base.HandleCollision();
        PlayerDetected = Physics2D.OverlapCircle(attackPoint.position, attackRadius, whatIsTarget);
    }

    public override void EnableMovement(bool enable) {
        if (!canAttack) return;
        base.EnableMovement(enable);
        
        if (enable == true)
            isAttacking = false;
    }
}
