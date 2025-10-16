using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public UnityEvent gameStart;
    public void OnStartClicked()
    {
        gameStart.Invoke();
    }
}
