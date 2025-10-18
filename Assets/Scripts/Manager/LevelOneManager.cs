using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelOneManager : MonoBehaviour
{
    public UnityEvent OnLevelOneTheme;
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
    public TMP_Text levelText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnLevelOneTheme.Invoke();
        initializeLevel();

    }

    // Update is called once per frame
    void Update()
    {


        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Get the current scene name
            string sceneName = SceneManager.GetActiveScene().name;

            // Only allow pause/resume in gameplay scenes
            if (sceneName.StartsWith("Level"))
            {
                if (!isPaused)
                    PauseGame();
                else
                    ResumeGame();
            }
        }

        if (timerRunning && !isPaused)
        {
            levelTimer += Time.unscaledDeltaTime;
            UpdateUI();
        }
    }

    public void onPooCollected()
    {
        Debug.Log("POOPOOCOLLECTED! called from levelmanager");

        totalPoos = Math.Max(totalPoos - 1, 0);

        if (totalPoos <= 0)
        {
            LevelComplete();
        }
    }

    public void onBatDeath()
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

    private void LevelComplete()
    {
        timerRunning = false;
        Debug.Log($"Level complete! Time: {levelTimer:F2}s");

        string levelName = SceneManager.GetActiveScene().name;
        int levelIndex = gameData.currentLevel - 1;

        // Ensure the array is large enough before accessing it
        if (levelIndex >= gameData.highScores.Length)
        {
            Array.Resize(ref gameData.highScores, levelIndex + 1);
            gameData.highScores[levelIndex] = -1f;
            Debug.Log($"Expanded highScores to length {gameData.highScores.Length}");
        }

        // Save new highscore if better
        if (gameData.highScores[levelIndex] < 0f || levelTimer < gameData.highScores[levelIndex])
        {
            gameData.highScores[levelIndex] = levelTimer;
            Debug.Log("New High Score!");
        }

        Debug.Log($"current high score: {gameData.highScores[levelIndex]} || levelTimer {levelTimer}");

        // update current run and total time
        gameData.currentRunTime = levelTimer;
        gameData.totalTime += levelTimer;

        // go to next level
        gameData.currentLevel++;
        gameData.SaveToPrefs();

        string nextLevelName = $"Level {gameData.currentLevel}";
        if (Application.CanStreamedLevelBeLoaded(nextLevelName))
            SceneManager.LoadScene(nextLevelName);
        else
            SceneManager.LoadScene("EndScreen");
    }

    public void PauseGame()
    {
        SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
        Time.timeScale = 0f;
        isPaused = true;
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

        levelText.text = $"Level {gameData.currentLevel}";

    }

    public void initializeLevel()
    {
        Transform poosParent = GameObject.Find("PooPoos")?.transform;
        totalPoos = poosParent != null ? poosParent.childCount : 0;

        levelTimer = 0f;
        timerRunning = true;

        timerText = GameObject.Find("TimerText")?.GetComponent<TMP_Text>();
        highscoreValueText = GameObject.Find("ScoreValue")?.GetComponent<TMP_Text>();
        UpdateUI();
    }

}
