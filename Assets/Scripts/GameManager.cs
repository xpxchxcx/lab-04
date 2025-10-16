using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("Game Data")]
    public GameData gameData;

    //[Header("Events")]
    //public UnityEvent levelComplete;
    //public UnityEvent stage1Complete;
    //public UnityEvent gameStart;
    //public UnityEvent gameOver;
    //public UnityEvent gamePaused;
    //public UnityEvent gameResumed;
    //public UnityEvent pooCollected;
    private void Start()
    {
        gameData.Initialize(maxLevels: 2);
        gameData.LoadFromPrefs();
    }

    private void Update()
    {

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
}
