using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData")]
public class GameData : ScriptableObject
{
    [Header("Progress")]
    public int currentLevel = 1;
    public float totalTime = 0f;

    [Header("High Scores")]
    public float[] highScores;

    [Header("Current Run Data")]
    public float currentRunTime = 0f;

    public void Initialize(int maxLevels)
    {
        if (highScores == null || highScores.Length != maxLevels)
            highScores = new float[maxLevels];

        // ensure clean defaults
        for (int i = 0; i < highScores.Length; i++)
        {
            if (float.IsNaN(highScores[i]) || highScores[i] == 0f || highScores[i] == float.MaxValue)
                highScores[i] = -1f;
        }
    }

    public void LoadFromPrefs()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        totalTime = PlayerPrefs.GetFloat("TotalTime", 0f);

        if (highScores == null)
            highScores = new float[2]; // fallback for first run

        for (int i = 0; i < highScores.Length; i++)
        {
            float score = PlayerPrefs.GetFloat($"HighScore_Level_{i + 1}", -1f);
            highScores[i] = score;
        }
    }

    public void SaveToPrefs()
    {
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.SetFloat("TotalTime", totalTime);

        for (int i = 0; i < highScores.Length; i++)
        {
            float value = highScores[i] < 0f ? -1f : highScores[i];
            PlayerPrefs.SetFloat($"HighScore_Level_{i + 1}", value);
        }

        PlayerPrefs.Save();
    }

    public void ResetData()
    {
        currentLevel = 1;
        totalTime = 0f;
        if (highScores == null)
            highScores = new float[2];
        for (int i = 0; i < highScores.Length; i++)
            highScores[i] = -1f;
        currentRunTime = 0f;
    }
}
