using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class FlashlightPowerup : MonoBehaviour, IPowerup
{
    [Header("Flashlight Powerup Settings")]
    public UnityEvent onPowerupCollected;
    public Light2D globalDarkness;
    public Color boostedColor = new Color(100f / 255f, 100f / 255f, 100f / 255f);
    public float brightDuration = 2f;
    public float fadeDuration = 1f;

    private SpriteRenderer sr;
    private Collider2D col;
    private bool _hasSpawned = false;

    public PowerupType powerupType => PowerupType.Flashlight;
    public bool hasSpawned => _hasSpawned;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        _hasSpawned = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            onPowerupCollected?.Invoke();

            // Apply effect through interface
            ApplyPowerup(other.GetComponent<MonoBehaviour>());
        }
    }

    public void ApplyPowerup(MonoBehaviour i)
    {
        if (globalDarkness == null) return;

        //Color originalColor = globalDarkness.color;
        //globalDarkness.color = boostedColor;

        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;
        i.GetComponent<BatStateController>().SetPowerup(PowerupType.Flashlight);
        //StartCoroutine(FadeBackToOriginal(globalDarkness, originalColor));
    }

    private IEnumerator FadeBackToOriginal(Light2D light, Color originalColor)
    {
        yield return new WaitForSeconds(brightDuration);

        Color startColor = light.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            light.color = Color.Lerp(startColor, originalColor, elapsed / fadeDuration);
            yield return null;
        }

        light.color = originalColor;
        DestroyPowerup();
    }

    public void DestroyPowerup()
    {
        Destroy(gameObject);
    }
}
