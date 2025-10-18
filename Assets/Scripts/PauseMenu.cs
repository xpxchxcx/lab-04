using UnityEngine;
using UnityEngine.Events;

public class PauseMenuUI : MonoBehaviour
{
    public UnityEvent OnResumeEvent;
    public UnityEvent OnQuitToMainMenuEvent;
    public void OnResumeClicked()
    {
        OnResumeEvent.Invoke();
    }

    public void OnQuitClicked()
    {
        OnQuitToMainMenuEvent.Invoke();
    }
}
