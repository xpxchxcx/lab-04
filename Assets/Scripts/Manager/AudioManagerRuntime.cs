using UnityEngine;

public class AudioManagerRuntime : MonoBehaviour
{
    public AudioManagerSO audioManagerSO;
    public AudioSource bgmSource;
    public AudioSource sfxSource;



    void Awake()
    {
        if (audioManagerSO != null)
            audioManagerSO.Initialize(bgmSource, sfxSource, this);
    }

    void Start()
    {

    }

    public void playPoo()
    {
        audioManagerSO?.PlayBigPoo();
    }

    public void playPowerup()
    {
        audioManagerSO.PlayPickup();
    }

    public void playHorror()
    {
        audioManagerSO?.PlayHorrorTheme();
    }

    public void levelTheme()
    {
        audioManagerSO?.PlayLevelTheme();
    }

    public void playSonar()
    {
        audioManagerSO?.PlaySonarPulse();
    }

    public void playMainMenuTheme()
    {
        audioManagerSO?.PlayMainMenuTheme();
    }

    public void playEndTheme()
    {
        audioManagerSO?.PlayEndTheme();
    }
}