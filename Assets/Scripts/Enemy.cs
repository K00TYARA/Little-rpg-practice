using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class Enemy : Entity {

    [Header("Movement details")]
    [SerializeField] protected float moveSpeed = 4;

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

    protected override void Move() {
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
    }

    protected override void HandleFlip() {
        int playerLayer = LayerMask.NameToLayer("Player");

        Transform[] all = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);

        Transform[] players = all
            .Where(en => en.gameObject.layer == playerLayer)
            .ToArray();

        Transform player = players[0];

        if (player.position.x < transform.position.x && facingRight == true && IsActionAndMovementAllowed() ||
            player.position.x > transform.position.x && facingRight == false && IsActionAndMovementAllowed()) {
            Flip();
        }
    }

    protected override void HandleCollision() {
        base.HandleCollision();
        PlayerDetected = Physics2D.OverlapCircle(attackPoint.position, attackRadius, whatIsTarget);
    }
}
