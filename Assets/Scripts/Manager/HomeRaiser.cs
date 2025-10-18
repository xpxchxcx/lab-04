using UnityEngine;
using UnityEngine.Events;

public class HomeRaiser : MonoBehaviour
{

    public UnityEvent onMainMenu;

    public void OnQuitClicked()
    {
        onMainMenu.Invoke();
    }
}
