using UnityEngine;

public class Player : MonoBehaviour {

    private Rigidbody2D rb;

    private float xInput;
    [SerializeField] private float moveSpeed = 2;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        xInput = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocityY);
    }
}