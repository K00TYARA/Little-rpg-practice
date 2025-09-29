using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class Enemy : Entity {

    [SerializeField] private bool PlayerDetected;
    private bool isAttackTriggerAdded = false;

    protected override void Awake() {
        base.Awake();
        isPlayer = false;
    }

    protected override void Update() {
        if (isDie || isHit) return;
        HandleCollision();
        HandleAnimation();
        HandleMovement();
        HandleFlip();
        HandleAttack();
    }

    protected override void HandleMovement() {
        if (canMove) {
            rb.linearVelocity = new Vector2(facingDir * moveSpeed, rb.linearVelocityY);
        } else {
            rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
        }
    }

    protected override void HandleAttack() {
        if (PlayerDetected && canAttack && !isAttackTriggerAdded) {
            animator.SetTrigger("attack");
            isAttackTriggerAdded = true;
        }
    }

    protected override void HandleAnimation() {
        animator.SetFloat("xVelocity", rb.linearVelocityX);
    }

    protected override void HandleCollision() {
        base.HandleCollision();
        PlayerDetected = Physics2D.OverlapCircle(attackPoint.position, attackRadius, whatIsTarget);
    }

    public void ChangeIsAttackTriggerAdded(bool enable) {
        isAttackTriggerAdded = enable;
    }

}
