using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    public void OnResumeClicked()
    {
        if (GameManager.instance != null)
            GameManager.instance.ResumeGame();
    }

    public void OnQuitClicked()
    {
        if (GameManager.instance != null)
            GameManager.instance.QuitToMainMenu();
    }
}
