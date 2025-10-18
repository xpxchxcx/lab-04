using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioManagerSO", menuName = "ScriptableObjects/AudioManagerSO")]
public class AudioManagerSO : ScriptableObject
{
    [Header("Mixer (optional)")]
    public AudioMixer mixer;
    public string bgmParam = "BGM_Volume";
    public string sfxParam = "SFX_Volume";

    [Header("Clips")]
    public AudioClip mainMenuTheme;
    public AudioClip victoryTheme;
    public AudioClip levelTheme;
    public AudioClip endTheme;
    public AudioClip bigPoo;
    public AudioClip horrorTheme;
    public AudioClip sonarPulse;
    public AudioClip pickup;





    [Header("Enemy Clips")]
    public AudioClip[] patrolClips;
    public AudioClip[] chaseClips;
    public AudioClip[] investigateClips;

    private Coroutine currentEnemyLoop;
    private AudioClip[] currentClipArray;

    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private MonoBehaviour coroutineHost;
    private Coroutine currentSFXCoroutine;

    // Must be called once by a runtime component to provide actual sources.
    public void Initialize(AudioSource bgm, AudioSource sfx, MonoBehaviour host)
    {
        bgmSource = bgm;
        sfxSource = sfx;
        coroutineHost = host;
    }

    // ------------------------------------
    // BGM
    // ------------------------------------
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null || bgmSource == null) return;
        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void PlayMainMenuTheme() => PlayBGM(mainMenuTheme, true);
    public void PlayVictoryTheme() => PlayBGM(victoryTheme, true);
    public void PlayLevelTheme() => PlayBGM(levelTheme, true);
    public void PlayHorrorTheme() => PlayBGM(horrorTheme, true);
    public void PlayEndTheme() => PlayBGM(endTheme, true);
    public void StopBGM() => bgmSource?.Stop();

    // ------------------------------------
    // SFX
    // ------------------------------------
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayPickup() => PlaySFX(pickup);
    public void PlayBigPoo() => PlaySFX(bigPoo);
    public void PlaySonarPulse() => PlaySFX(sonarPulse);

    // ------------------------------------
    // Mixer
    // ------------------------------------
    public void SetBGMVolume(float v) => SetDb(bgmParam, v);
    public void SetSFXVolume(float v) => SetDb(sfxParam, v);

    private void SetDb(string param, float linear01)
    {
        if (mixer == null) return;
        float db = Mathf.Log10(Mathf.Clamp(linear01, 0.0001f, 1f)) * 20f;
        mixer.SetFloat(param, db);
    }

    // ------------------------------------
    // Utility
    // ------------------------------------
    public void FadeAudio(AudioSource src, float targetVol, float duration)
    {
        if (coroutineHost != null)
            coroutineHost.StartCoroutine(FadeAudioCoroutine(src, targetVol, duration));
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

    public void PlayEnemyLoop(AudioSource targetSource, AudioClip[] clips, float minInterval = 2f, float maxInterval = 5f)
    {
        if (clips == null || clips.Length == 0 || targetSource == null || coroutineHost == null) return;

        // If already playing this set, do nothing
        if (currentClipArray == clips) return;

        // Stop previous loop
        if (currentEnemyLoop != null)
            coroutineHost.StopCoroutine(currentEnemyLoop);

        currentClipArray = clips;
        currentEnemyLoop = coroutineHost.StartCoroutine(PlayEnemyLoopCoroutine(targetSource, clips, minInterval, maxInterval));
    }

    private IEnumerator PlayEnemyLoopCoroutine(AudioSource source, AudioClip[] clips, float minInterval, float maxInterval)
    {
        while (true)
        {
            var clip = clips[Random.Range(0, clips.Length)];
            source.PlayOneShot(clip);
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
        }
    }

    public void StopEnemyLoop()
    {
        if (currentEnemyLoop != null && coroutineHost != null)
        {
            coroutineHost.StopCoroutine(currentEnemyLoop);
            currentEnemyLoop = null;
            currentClipArray = null;
        }
    }
}
