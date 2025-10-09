using UnityEngine;
using TMPro;

public class EndScreenManager : MonoBehaviour
{
    public TMP_Text highScoresText;
    public GameData gameData;
    void Start()
    {
        AudioManager.I.PlayVictoryTheme();

        string display = "";
        for (int i = 0; i < gameData.highScores.Length; i++)
        {
            float high = gameData.highScores[i];
            string highScoreText = (high < 0f) ? "N/A" : $"{high:F2}s";
            display += $"Level {i + 1} High Score: {highScoreText}\n";
        }

        display += $"\nTotal Time: {gameData.totalTime:F2}s";
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
