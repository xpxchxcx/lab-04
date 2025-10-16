using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Level1Manager : MonoBehaviour
{
    [Header("Game Data")]
    public GameData gameData;

    [Header("Pause Menu")]
    private bool isPaused = false;

    [Header("Poo Tracking")]
    public int totalPoos = 0;

    [Header("Timer")]
    private float levelTimer = 0f;
    private bool timerRunning = false;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text highscoreValueText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform poosParent = GameObject.Find("PooPoos")?.transform;
        totalPoos = poosParent != null ? poosParent.childCount : 0;

        levelTimer = 0f;
        timerRunning = true;

        timerText = GameObject.Find("TimerText")?.GetComponent<TMP_Text>();
        highscoreValueText = GameObject.Find("ScoreValue")?.GetComponent<TMP_Text>();
        UpdateUI();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }

        if (timerRunning && !isPaused)
        {
            levelTimer += Time.unscaledDeltaTime;
            UpdateUI();
        }
    }

    public void OnPooCollected()
    {
        totalPoos = Mathf.Max(totalPoos - 1, 0);
        UpdateUI();

        if (totalPoos <= 0)
        {
            SceneManager.LoadScene("Level 2");
        }
    }

    public void PauseGame()
    {
        SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void OnBatDeath()
    {
        gameData.currentRunTime = 0f;
        SceneManager.LoadScene("EndScreen");
    }
    public void ResumeGame()
    {
        Debug.Log("resume  called");
        SceneManager.UnloadSceneAsync("PauseMenu");
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitToMainMenu()
    {
        AudioManager.I.PlayMainMenuTheme();
        Debug.Log("Quit to main menu called");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        isPaused = false;
    }

    private void UpdateUI()
    {
        if (timerText != null)
            timerText.text = $"Time: {levelTimer:F2}s\nPoo Left: {totalPoos}";

        float totalHigh = 0f;
        bool hasHigh = false;

        for (int i = 0; i < gameData.highScores.Length; i++)
        {
            if (gameData.highScores[i] >= 0f)
            {
                totalHigh += gameData.highScores[i];
                hasHigh = true;
            }
        }

        if (highscoreValueText != null)
            highscoreValueText.text = $"{(hasHigh ? $"{totalHigh:F2}s" : "None")}";

    }
}
