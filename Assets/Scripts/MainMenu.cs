using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public UnityEvent OnStartGameEvent;
    public void OnStartClicked()
    {
        OnStartGameEvent.Invoke();
    }
}
