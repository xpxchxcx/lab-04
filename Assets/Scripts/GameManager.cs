using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public override void Awake()
    {
        base.Awake();
    }


    public void onStartButton()
    {
        SceneManager.LoadSceneAsync("Level 1", LoadSceneMode.Single);
    }
}
