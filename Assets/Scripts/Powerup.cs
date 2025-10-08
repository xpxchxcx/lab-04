using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Powerup : MonoBehaviour
{
    public Light2D globalDarkness;
    public Color boostedColor = new Color(100f / 255f, 100f / 255f, 100f / 255f);
    public float brightDuration = 2f; // time light stays fully bright
    public float fadeDuration = 1f;   // time to fade back

    private SpriteRenderer sr;
    private Collider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Store original color
            Color originalColor = globalDarkness.color;

            // Set boosted color instantly
            globalDarkness.color = boostedColor;

            if (sr != null) sr.enabled = false;
            if (col != null) col.enabled = false;

            // Start coroutine to fade back smoothly
            StartCoroutine(FadeBackToOriginal(globalDarkness, originalColor));
        }
    }
    private IEnumerator FadeBackToOriginal(Light2D light, Color originalColor)
    {
        // Wait for fully bright duration
        yield return new WaitForSeconds(brightDuration);

        Color startColor = light.color;
        float elapsed = 0f;

        // Smoothly interpolate back to original color
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            light.color = Color.Lerp(startColor, originalColor, elapsed / fadeDuration);
            yield return null;
        }

        // Ensure exact original color
        light.color = originalColor;

        // Destroy powerup
        Destroy(gameObject);
    }
}
