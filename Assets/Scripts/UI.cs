using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    public static UI instance;

    [SerializeField] private GameObject gameOverUI;
    [Space]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI killCountText;
    [SerializeField] private Image dashSkillImage;
    [SerializeField] private Image dashSkillVisualTimer;
    [SerializeField] public Image healthBar;

    private int killCount = 0;
    private float elapsedTime = 0f;
    private bool isTimerActive = true;

    private void Awake() {
        instance = this;
        Time.timeScale = 1;
    }

    private void Update() {
        HandleGameTimer();
    }

    public void EnableGameOverUI() {
        Time.timeScale = .6f;
        gameOverUI.SetActive(true);
        isTimerActive = false;
    } 

    private void HandleGameTimer() {
        if (isTimerActive) {
            timerText.text = elapsedTime.ToString("F1") + "s";
            elapsedTime += Time.deltaTime;
        }
    }

    public void ShowDashCooldown(float dashCooldownDuration) {
        StartCoroutine(HandleDashCooldown(dashCooldownDuration));
    }

    private IEnumerator HandleDashCooldown(float dashCooldownDuration) {
        dashSkillVisualTimer.gameObject.SetActive(true);
        dashSkillImage.color = new Color(166f/255f, 166f/255f, 166f/255f);
        float dashCooldownTimer = dashCooldownDuration;

        while (dashCooldownTimer > 0) {
            dashSkillVisualTimer.fillAmount = dashCooldownTimer / dashCooldownDuration;
            dashCooldownTimer -= Time.deltaTime;
            yield return null;
        }

        dashSkillVisualTimer.gameObject.SetActive(false);
        dashSkillImage.color = Color.white;
    }

    public void HandleHealthBarVisual(float currentHealth, float maxHealth) {
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    public void AddKillCount() => killCountText.text = (++killCount).ToString();
    public void RestartLevel() {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
        elapsedTime = 0f;
        isTimerActive = true;
        Entity.isGameOver = false;
    }
}
