using UnityEngine;
using TMPro;

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
            string currentKey = $"CurrentRun_Level {i}";

            if (PlayerPrefs.HasKey(highKey))
            {
                float highScore = PlayerPrefs.GetFloat(highKey);
                display += $"Level {i} High Score: {highScore:F2}s";

                if (PlayerPrefs.HasKey(currentKey))
                {
                    float currentScore = PlayerPrefs.GetFloat(currentKey);
                    display += $" (This Run: {currentScore:F2}s)";
                }

                display += "\n";
            }
        }

        // Display cumulative time
        float totalTime = PlayerPrefs.GetFloat("CurrentTime", 0f);
        display += $"\nTotal Time: {totalTime:F2}s";

        highScoresText.text = display;
    }
}
