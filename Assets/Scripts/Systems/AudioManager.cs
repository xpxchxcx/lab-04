using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }
    private float pitchShiftValue = 1f;

    [Header("Mixer (optional)")]
    public AudioMixer mixer;                 // drag MainMixer if you want sliders
    public string bgmParam = "BGM_Volume";   // exposed param names in Mixer
    public string sfxParam = "SFX_Volume";

    [Header("Sources (drag from Hierarchy)")]
    [SerializeField] private AudioSource bgmSource;   // drag child BGM
    [SerializeField] private AudioSource sfxSource;   // drag child SFX

    [Header("Clips (drag here)")]
    public AudioClip horrorTheme;
    public AudioClip levelTheme;
    public AudioClip flying;
    public AudioClip teoEnMing;
    public AudioClip enemyHit;
    public AudioClip pickup;
    public AudioClip bigPoop;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);  // remove if you don’t want it persistent
    }

    // --- BGM ---
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null || bgmSource == null) return;
        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }
    public void StopBGM() { if (bgmSource != null) bgmSource.Stop(); }

    // Convenience hooks for UnityEvents / buttons:
    public void PlayHorrorTheme() => PlayBGM(horrorTheme, true);
    public void PlayLevelTheme() => PlayBGM(levelTheme, true);


    // --- SFX (simple, 2D) ---
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    // Convenience hooks:
    public void PlayFlying() => PlaySFX(flying);
    public void PlayEnemyHit() => PlaySFX(enemyHit);
    public void PlayPickup() => PlaySFX(pickup);
    public void PlayEnMingTalk() => PlaySFX(teoEnMing);
    public void PlayBigPoop() => PlaySFX(bigPoop);

    // --- Mixer sliders (0..1 linear) ---
    public void SetBGMVolume(float v) => SetDb(bgmParam, v);
    public void SetSFXVolume(float v) => SetDb(sfxParam, v);

    private void SetDb(string param, float linear01)
    {
        if (mixer == null || string.IsNullOrEmpty(param)) return;
        float db = Mathf.Log10(Mathf.Clamp(linear01, 0.0001f, 1f)) * 20f;
        mixer.SetFloat(param, db);
    }


    void Start()
    {
        PlayLevelTheme();
        //GoombaDieManager.goombaDieEvent += PlayEnemyHit;
        //GoombaDieManager.goombaDieEvent += pitchShiftOnGoombaDie;
        //GoombaDieManager.goombaDieEvent += PlayBigPoop;
        //PlayerMovement.PlayerJumpEvent += PlayJump;
        //PoopGun.PoopGunShoot += PlayShoot;
    }


}
