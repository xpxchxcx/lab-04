using UnityEngine;
using TMPro;

public class EndScreenManager : MonoBehaviour
{
    [Header("Per-Level Texts")]
    public TMP_Text[] levelHighScoreTexts;
    public TMP_Text[] levelCurrentScoreTexts;

    [Header("Totals")]
    public TMP_Text totalHighScoreText;
    public TMP_Text totalCurrentScoreText;

    [Header("Message")]
    public TMP_Text statusMessage; // e.g., “You Died!” or “All Levels Complete!”

    [Header("Data")]
    public GameData gameData;

    void Start()
    {
        AudioManager.I.PlayVictoryTheme();

        bool died = gameData.currentRunTime <= 0f; // if 0, player died
        bool finishedAll = gameData.currentLevel > gameData.highScores.Length;

        // --- Message at top ---
        if (statusMessage != null)
        {
            if (died)
                statusMessage.text = "You Died!";
            else if (finishedAll)
                statusMessage.text = "All Levels Complete!";
            else
                statusMessage.text = "Level Complete!";
        }

        // --- Per-level scores ---
        for (int i = 0; i < gameData.highScores.Length; i++)
        {
            string levelLabel = $"Level {i + 1}";

            // High scores (always shown)
            if (i < levelHighScoreTexts.Length && levelHighScoreTexts[i] != null)
            {
                float high = gameData.highScores[i];
                string highScoreText = (high < 0f) ? "N/A" : $"{high:F2}s";
                levelHighScoreTexts[i].text = $"{levelLabel} High Score: {highScoreText}";
            }

            // Current run (only if reached)
            if (i < levelCurrentScoreTexts.Length && levelCurrentScoreTexts[i] != null)
            {
                string currentScoreText = "N/A";

                // if player completed this level successfully this run
                if (!died && i + 1 < gameData.currentLevel)
                    currentScoreText = $"{gameData.currentRunTime:F2}s";

                levelCurrentScoreTexts[i].text = $"{levelLabel} This Run: {currentScoreText}";
            }
        }

        // --- Totals ---
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

        if (totalHighScoreText != null)
            totalHighScoreText.text = $"Total High Score: {(hasHigh ? $"{totalHigh:F2}s" : "N/A")}";

        if (totalCurrentScoreText != null)
        {
            if (died)
                totalCurrentScoreText.text = "Total Current: Died";
            else
                totalCurrentScoreText.text = $"Total Current: {(gameData.totalTime > 0f ? $"{gameData.totalTime:F2}s" : "N/A")}";
        }
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
