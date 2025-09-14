using UnityEngine;

public class Player : MonoBehaviour {

    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        rb.linearVelocity = new Vector2(Input.GetAxis("Horizontal"), rb.linearVelocityY);
    }
}