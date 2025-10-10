using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Game Data")]
    public GameData gameData;
    public static GameManager instance;

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


    private void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            isPaused = false;
            DontDestroyOnLoad(gameObject);
            gameData.Initialize(maxLevels: 2);
            gameData.LoadFromPrefs();
        }
        else
        {
            Destroy(gameObject);
        }
    }



    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Poo.OnPooCollected += OnPooCollected;
        TopDownBatController.OnBatDeath += OnBatDeath;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Poo.OnPooCollected -= OnPooCollected;
        TopDownBatController.OnBatDeath -= OnBatDeath;
    }

    private void OnPooCollected()
    {
        totalPoos = Mathf.Max(totalPoos - 1, 0);
        UpdateUI();

        if (totalPoos <= 0)
        {
            LevelComplete();
        }
    }

    private void Update()
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        // Only initialize gameplay scenes
        if (sceneName.StartsWith("Level"))
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

    public void PauseGame()
    {
        SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
        Time.timeScale = 0f;
        isPaused = true;
    }

    private void OnBatDeath()
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

    public void StartGame()
    {
        // Only reset progress for a new run, not high scores
        gameData.currentLevel = 1;
        gameData.totalTime = 0f;
        gameData.currentRunTime = 0f;
        gameData.isPause = false;

        // Keep previous highscores intact
        gameData.SaveToPrefs();

        SceneManager.LoadScene("Level 1");
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

    private void LevelComplete()
    {
        timerRunning = false;
        Debug.Log($"Level complete! Time: {levelTimer:F2}s");

        string levelName = SceneManager.GetActiveScene().name;
        int levelIndex = gameData.currentLevel - 1;

        // update high score if new best
        if (gameData.highScores[levelIndex] < 0f || levelTimer < gameData.highScores[levelIndex])
        {
            gameData.highScores[levelIndex] = levelTimer;
            Debug.Log("New High Score!");
        }

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
}
