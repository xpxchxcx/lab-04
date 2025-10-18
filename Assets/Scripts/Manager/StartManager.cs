using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public GameData gameData;
    public UnityEvent OnMainMenuEvent;
    void Start()
    {
        OnMainMenuEvent.Invoke();
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
