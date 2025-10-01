using UnityEngine;


public class Player : Entity {

    private float xInput;

    [Header("Movement details")]
    [SerializeField] protected float moveSpeed = 4;
    [SerializeField] protected float jumpForce = 8;
    
    protected bool canJump = true;

    protected override void Awake() {
        base.Awake();
        isPlayer = true;
    }

    protected override void Update() {
        if (isDie || isHit) return;
        base.Update();
        HandleInput();
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

    protected override void HandleMovement() {
        if (canMove)
            rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocityY);
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
    }

    private void TryToJump() {
        if (isGrounded && canJump) {
            rb.gravityScale = 2.5f;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        }
    }

    public override void EnableMovement(bool enable) {
        if (!canAttack) return;
        base.EnableMovement(enable);
        canJump = enable;
    }

}
