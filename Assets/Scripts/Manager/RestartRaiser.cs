using UnityEngine;
using UnityEngine.Events;

public class RestartRaiser : MonoBehaviour
{
    public UnityEvent onRestart;
    public void OnRestartClicked()
    {
        onRestart.Invoke();
    }

}
