using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public void OnStartClicked()
    {
        if (GameManager.instance != null)
            GameManager.instance.StartGame();
    }
}
