using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject pauseMenu; // Only assign when in Level1
    private bool isPaused = false;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Subscribe to scene loaded event
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (pauseMenu != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!isPaused) PauseGame();
            else ResumeGame();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Assign PauseMenu if we are in Level1
        if (scene.name == "Level 1")
        {
            pauseMenu = GameObject.Find("PauseMenu");

            Button resumeButton = pauseMenu.transform.Find("ResumeButton").GetComponent<Button>();
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeGame);

            Button quitButton = pauseMenu.transform.Find("QuitButton").GetComponent<Button>();
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitToMainMenu);

            pauseMenu.SetActive(false);
        }
        else
        {
            pauseMenu = null; // no pause menu in MainMenu
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
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
        SceneManager.LoadScene("Level 1");
    }
}
