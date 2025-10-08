using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public GameObject endScreen;
    private bool isPaused = false;

    [Header("Poo Tracking")]
    public int totalPoos = 0;

    [Header("Timer")]
    private float levelTimer = 0f;
    private bool timerRunning = false;

    [Header("UI")]
    public TMP_Text timerText;

    private bool levelCompleted = false;

    private void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
            levelCompleted = true;
            LevelComplete();
        }
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!isPaused) PauseGame();
            else ResumeGame();
        }

        if (timerRunning && !isPaused)
        {
            levelTimer += Time.unscaledDeltaTime;
            UpdateUI();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        pauseMenu = GameObject.Find("PauseMenu");
        endScreen = GameObject.Find("EndScreen");
        if (pauseMenu != null)
        {
            Button resumeButton = pauseMenu.transform.Find("ResumeButton")?.GetComponent<Button>();
            resumeButton?.onClick.RemoveAllListeners();
            resumeButton?.onClick.AddListener(ResumeGame);

            Button quitButton = pauseMenu.transform.Find("QuitButton")?.GetComponent<Button>();
            quitButton?.onClick.RemoveAllListeners();
            quitButton?.onClick.AddListener(QuitToMainMenu);

            pauseMenu.SetActive(false);
        }
        if (endScreen)
        {
            Button mainMenuButton = endScreen.transform.Find("HomeButton")?.GetComponent<Button>();
            mainMenuButton?.onClick.RemoveAllListeners();
            mainMenuButton?.onClick.AddListener(QuitToMainMenu);

        }

        Transform poosParent = GameObject.Find("PooPoos")?.transform;
        totalPoos = poosParent != null ? poosParent.childCount : 0;

        levelTimer = 0f;
        timerRunning = true;

        timerText = GameObject.Find("TimerText")?.GetComponent<TMP_Text>();
        UpdateUI();
    }

    public void PauseGame()
    {
        if (pauseMenu != null) pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    private void OnBatDeath()
    {
        levelCompleted = false;         // mark as failed
        timerRunning = false;
        levelTimer = 0f;                // this run = 0
        Debug.Log("Player died. Level failed. Run score = 0");

        // Save 0 for this run so EndScreen shows 0
        string levelName = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetFloat($"CurrentRun_{levelName}", 0f);
        PlayerPrefs.Save();

        SceneManager.LoadScene("EndScreen");
    }

    public void ResumeGame()
    {
        Debug.Log("resume  called");

        if (pauseMenu != null) pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitToMainMenu()
    {
        Debug.Log("Quit to main menu called");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void StartGame()
    {
        PlayerPrefs.SetFloat("CurrentTime", 0f);
        PlayerPrefs.SetInt("CurrentLevel", 1);
        SceneManager.LoadScene("Level 1");
    }

    private void UpdateUI()
    {
        if (timerText != null)
            timerText.text = $"Time: {levelTimer:F2}s\nPoo Left: {totalPoos}";
    }

    private void LevelComplete()
    {
        timerRunning = false;
        Debug.Log($"Level complete! Time: {levelTimer:F2}s");

        if (!levelCompleted)
        {
            Debug.Log("Level failed. Score ignored.");
            return; // don't save anything
        }

        string levelName = SceneManager.GetActiveScene().name;

        // Save all-time high score
        string levelKey = $"HighScore_{levelName}";
        float prevBest = PlayerPrefs.GetFloat(levelKey, float.MaxValue);
        if (levelTimer < prevBest)
        {
            PlayerPrefs.SetFloat(levelKey, levelTimer);
            Debug.Log("New High Score!");
        }

        // Save this run
        PlayerPrefs.SetFloat($"CurrentRun_{levelName}", levelTimer);

        // Update cumulative time
        float totalTime = PlayerPrefs.GetFloat("CurrentTime", 0f);
        totalTime += levelTimer;
        PlayerPrefs.SetFloat("CurrentTime", totalTime);

        // Next level
        int nextLevel = PlayerPrefs.GetInt("CurrentLevel", 1) + 1;
        PlayerPrefs.SetInt("CurrentLevel", nextLevel);
        PlayerPrefs.Save();

        string nextLevelName = $"Level {nextLevel}";
        if (Application.CanStreamedLevelBeLoaded(nextLevelName))
            SceneManager.LoadScene(nextLevelName);
        else
            SceneManager.LoadScene("EndScreen");
    }
}
