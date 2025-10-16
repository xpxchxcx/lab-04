using UnityEngine;
using UnityEngine.Events;

public class PauseMenuUI : MonoBehaviour
{
    public UnityEvent gameQuit;
    public UnityEvent gameResumed;

    public void OnResumeClicked()
    {
        gameResumed.Invoke();
    }

    public void OnQuitClicked()
    {
        gameQuit.Invoke();
    }
}
