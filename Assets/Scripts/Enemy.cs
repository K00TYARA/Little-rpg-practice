using UnityEngine;

public class Enemy : MonoBehaviour {

    private SpriteRenderer sr;

    [SerializeField] private float RedColorDuration = 1;
    public float lastTimeDamaged;

    private Color thisColor;

    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
        thisColor = sr.color;
    }

    private void Update() {
        ChangeColorIfNeeded();
    }

    private void ChangeColorIfNeeded() {
        if (sr.color == Color.red && Time.time > lastTimeDamaged + RedColorDuration)
            sr.color = thisColor;
    }

    public void TakeDamage() {
        sr.color = Color.red;
        lastTimeDamaged = Time.time;
    }

}
