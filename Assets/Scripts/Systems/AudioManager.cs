using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [Header("Mixer (optional)")]
    public AudioMixer mixer;
    public string bgmParam = "BGM_Volume";
    public string sfxParam = "SFX_Volume";

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip levelTheme;
    public AudioClip horrorTheme;

    [Header("General SFX Clips")]
    public AudioClip enemyHit;
    public AudioClip pickup;
    public AudioClip bigPoo;

    [Header("Enemy Clips")]
    public AudioClip[] patrolClips;
    public AudioClip[] chaseClips;
    public AudioClip[] investigateClips;

    [HideInInspector] public AudioClip[] CurrentClipArray;

    private Coroutine currentSFXCoroutine;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PlayBGM(levelTheme, true);
    }

    // -----------------------------
    // BGM Functions
    // -----------------------------
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null || bgmSource == null) return;
        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }
    public AudioSource GetBGMSource() => bgmSource;

    public void StopBGM() => bgmSource?.Stop();

    // -----------------------------
    // SFX Functions
    // -----------------------------
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public IEnumerator PlayRandomSFXCoroutine(AudioClip[] clips, float minInterval, float maxInterval)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            if (clips.Length > 0)
            {
                AudioClip clip = clips[Random.Range(0, clips.Length)];
                sfxSource.pitch = Random.Range(0.9f, 1.1f);
                sfxSource.volume = Random.Range(0.7f, 1f);
                sfxSource.PlayOneShot(clip);
            }
        }
    }

    public void StopRandomSFX()
    {
        if (currentSFXCoroutine != null)
        {
            StopCoroutine(currentSFXCoroutine);
            currentSFXCoroutine = null;
        }
    }

    // -----------------------------
    // Mixer Controls
    // -----------------------------
    public void SetBGMVolume(float v) => SetDb(bgmParam, v);
    public void SetSFXVolume(float v) => SetDb(sfxParam, v);

    private void SetDb(string param, float linear01)
    {
        if (mixer == null) return;
        float db = Mathf.Log10(Mathf.Clamp(linear01, 0.0001f, 1f)) * 20f;
        mixer.SetFloat(param, db);
    }

    // -----------------------------
    // Utility
    // -----------------------------
    public void FadeAudio(AudioSource src, float targetVol, float duration)
    {
        I.StartCoroutine(FadeAudioCoroutine(src, targetVol, duration));
    }

    private IEnumerator FadeAudioCoroutine(AudioSource src, float targetVol, float duration)
    {
        float startVol = src.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            src.volume = Mathf.Lerp(startVol, targetVol, t / duration);
            yield return null;
        }
        src.volume = targetVol;
    }

    public void PlayBigPoop() => PlaySFX(bigPoo);


    // =====================================================
    // STATE-BASED SOUND HELPERS (Now supports custom sources)
    // =====================================================
    public void PlayEnemyPatrolLoop(AudioSource source = null)
    {
        SwitchEnemyLoop(source ?? sfxSource, patrolClips, 3f, 6f);
    }

    public void PlayEnemyChaseLoop(AudioSource source = null)
    {
        SwitchEnemyLoop(source ?? sfxSource, chaseClips, 1f, 3f);
    }

    public void PlayEnemyInvestigateLoop(AudioSource source = null)
    {
        SwitchEnemyLoop(source ?? sfxSource, investigateClips, 2f, 5f);
    }

    // Core loop controller
    private void SwitchEnemyLoop(AudioSource source, AudioClip[] clips, float minInterval, float maxInterval)
    {
        // If we’re already using this clip array on this source, skip
        if (CurrentClipArray == clips && currentSFXCoroutine != null) return;

        // Stop old loop coroutine (if any)
        if (currentSFXCoroutine != null)
        {
            StopCoroutine(currentSFXCoroutine);
            currentSFXCoroutine = null;
        }

        // Remember which clips are active
        CurrentClipArray = clips;

        // Start playing on the provided source
        currentSFXCoroutine = StartCoroutine(PlayRandomSFXCoroutine(source, clips, minInterval, maxInterval));
    }

    // Version that uses custom AudioSource
    public IEnumerator PlayRandomSFXCoroutine(AudioSource source, AudioClip[] clips, float minInterval, float maxInterval)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            if (clips.Length > 0 && source != null)
            {
                AudioClip clip = clips[Random.Range(0, clips.Length)];
                source.pitch = Random.Range(0.9f, 1.1f);
                source.volume = Random.Range(0.7f, 1f);
                source.PlayOneShot(clip);
            }
        }
    }

}
