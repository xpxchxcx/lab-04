using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button startButton;

    void Start()
    {
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(() => GameManager.instance.StartGame());
    }
}
