using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenManager : MonoBehaviour
{
    public TMP_Text highScoresText;

    void Start()
    {
        string display = "";
        int maxLevels = 2;

        for (int i = 1; i <= maxLevels; i++)
        {
            string highKey = $"HighScore_Level {i}";
            string runKey = $"CurrentRun_Level {i}";

            float highScore = PlayerPrefs.HasKey(highKey) ? PlayerPrefs.GetFloat(highKey) : 0f;
            float thisRun = PlayerPrefs.HasKey(runKey) ? PlayerPrefs.GetFloat(runKey) : 0f;

            display += $"Level {i} High Score: {highScore:F2}s (This Run: {thisRun:F2}s)\n";
        }

        float totalTime = PlayerPrefs.GetFloat("CurrentTime", 0f);
        display += $"\nTotal Time: {totalTime:F2}s";



        highScoresText.text = display;
    }

    public void OnRestartClicked()
    {
        if (GameManager.instance != null)
            GameManager.instance.StartGame();
    }

    public void OnQuitClicked()
    {
        if (GameManager.instance != null)
            GameManager.instance.QuitToMainMenu();
    }
}
