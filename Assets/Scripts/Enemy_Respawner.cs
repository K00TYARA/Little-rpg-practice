using UnityEngine;

public class Enemy_Respawner : MonoBehaviour {

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] respawnPoints;
    [SerializeField] private float cooldown = 2f;
    [Space]
    [SerializeField] private float cooldownDecreaseRate = 0.05f;
    [SerializeField] private float cooldownCap = 0.7f;
    private Entity entity;
    private float timer;

    private void Awake() {
        entity = FindFirstObjectByType<Entity>();
    }

    private void Update() {
        if (entity.isGameOver) return;
        timer -= Time.deltaTime;

        if (timer <= 0) {
            timer = cooldown;
            CreateNewEnemy();

            cooldown = Mathf.Max(cooldown - cooldownDecreaseRate, cooldownCap);
        }
    }

    private void CreateNewEnemy() {
        int respawnPointIndex = Random.Range(0, respawnPoints.Length);
        Vector3 spawnPoint = respawnPoints[respawnPointIndex].position;

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
    }
}
