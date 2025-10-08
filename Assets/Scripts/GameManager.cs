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
    private bool isPaused = false;

    [Header("Poo Tracking")]
    public int totalPoos = 0;

    [Header("Timer")]
    private float levelTimer = 0f;
    private bool timerRunning = false;

    [Header("UI")]
    public TMP_Text timerText;

    private void Awake()
    {
        // Singleton pattern
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
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Poo.OnPooCollected -= OnPooCollected;
    }

    private void OnPooCollected()
    {
        totalPoos = Mathf.Max(totalPoos - 1, 0);
        UpdateUI();

        if (totalPoos <= 0)
            LevelComplete();
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
            levelTimer += Time.unscaledDeltaTime; // ignore Time.timeScale
            UpdateUI();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        // Setup pause menu if it exists
        pauseMenu = GameObject.Find("PauseMenu");
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

        // Count poos in current scene
        Transform poosParent = GameObject.Find("PooPoos")?.transform;
        totalPoos = poosParent != null ? poosParent.childCount : 0;

        // Reset timer
        levelTimer = 0f;
        timerRunning = true;

        // Assign UI
        timerText = GameObject.Find("TimerText")?.GetComponent<TMP_Text>();
        UpdateUI();
    }

    public void PauseGame()
    {
        if (pauseMenu != null) pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void StartGame()
    {
        PlayerPrefs.SetFloat("CurrentTime", 0f); // Reset cumulative timer
        PlayerPrefs.SetInt("CurrentLevel", 1);
        SceneManager.LoadScene("Level 1");


        Debug.Log("HighScore_Level 1: " + PlayerPrefs.GetFloat("HighScore_Level 1", 1f));
        Debug.Log("HighScore_Level 2: " + PlayerPrefs.GetFloat("HighScore_Level 2", 1f));
    }


    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
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

        string levelName = SceneManager.GetActiveScene().name;

        // Save all-time high score
        string levelKey = $"HighScore_{levelName}";
        float prevBest = PlayerPrefs.GetFloat(levelKey, float.MaxValue);
        if (levelTimer < prevBest)
        {
            PlayerPrefs.SetFloat(levelKey, levelTimer);
            Debug.Log("New High Score!");
        }

        // Save this run score
        string currentRunKey = $"CurrentRun_{levelName}";
        PlayerPrefs.SetFloat(currentRunKey, levelTimer);

        // Update cumulative time
        float totalTime = PlayerPrefs.GetFloat("CurrentTime", 0f);
        totalTime += levelTimer;
        PlayerPrefs.SetFloat("CurrentTime", totalTime);

        // Determine next level
        int nextLevel = PlayerPrefs.GetInt("CurrentLevel", 1) + 1;
        PlayerPrefs.SetInt("CurrentLevel", nextLevel);
        PlayerPrefs.Save();

        // Load next level or EndScreen
        string nextLevelName = $"Level {nextLevel}";
        if (Application.CanStreamedLevelBeLoaded(nextLevelName))
            SceneManager.LoadScene(nextLevelName);
        else
            SceneManager.LoadScene("EndScreen");
    }

}
