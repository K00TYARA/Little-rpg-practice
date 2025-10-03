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
    [SerializeField] public TextMeshProUGUI DashSkillTimer;
    [SerializeField] public Image DashSkillImage;
    [SerializeField] public Image HealthBar;

    private int killCount = 0;
    private float elapsedTime = 0f;
    private bool isTimerActive = true;

    private void Awake() {
        instance = this;
        Time.timeScale = 1;
    }

    public void EnablegameOverUI() {
        Time.timeScale = .6f;
        gameOverUI.SetActive(true);
        isTimerActive = false;
    } 
    
    private void Update() {
        if (isTimerActive) {
            timerText.text = elapsedTime.ToString("F1") + "s";
            elapsedTime += Time.deltaTime;
        }
    }

    public void RestartLevel() {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
        elapsedTime = 0f;
        isTimerActive = true;
    }

    public void addKillCount() => killCountText.text = (++killCount).ToString();
}
