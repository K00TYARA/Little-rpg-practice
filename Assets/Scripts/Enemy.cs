using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class Enemy : Entity {

    protected override void Update() {
        HandleCollision();
        HandleAnimation();
        HandleMovement();
        HandleFlip();
    }

    protected override void HandleMovement() {
        if (canMove)
            rb.linearVelocity = new Vector2(facingDir * moveSpeed, rb.linearVelocityY);
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
    }

    protected override void HandleAnimation() {
        animator.SetFloat("xVelocity", rb.linearVelocityY);
    }

}
