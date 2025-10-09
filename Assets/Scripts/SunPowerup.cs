using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SunPowerup : MonoBehaviour
{
    public ParticleSystem particleSystemPrefab;
    public float startRate = 20f;
    public float boostedRate = 50f;
    public float duration = 10f;

    private SpriteRenderer sr;
    private Collider2D col;
    private ParticleSystem ps;
    private ParticleSystem.EmissionModule emission;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        ps = particleSystemPrefab;
        emission = ps.emission;

        var rate = emission.rateOverTime;
        rate = startRate;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.I.PlayPickup();
            if (sr != null) sr.enabled = false;
            if (col != null) col.enabled = false;

            var rate = emission.rateOverTime;
            rate.constant = boostedRate;
            emission.rateOverTime = rate;

            StartCoroutine(RevertEmissionRate());
        }
    }
    private IEnumerator RevertEmissionRate()
    {
        yield return new WaitForSeconds(duration);

        // Instantly set emission back to startRate
        var rate = emission.rateOverTime;
        rate.constant = startRate;
        emission.rateOverTime = rate;

        // Destroy powerup
        Destroy(gameObject);
    }
}
