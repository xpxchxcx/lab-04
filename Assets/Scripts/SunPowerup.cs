using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SunPowerup : MonoBehaviour, IPowerup
{
    [Header("Sun Powerup Settings")]
    public UnityEvent onPowerupCollected;
    public ParticleSystem particleSystemPrefab;
    public float startRate = 20f;
    public float boostedRate = 50f;
    public float duration = 10f;

    private SpriteRenderer sr;
    private Collider2D col;
    private ParticleSystem ps;
    private ParticleSystem.EmissionModule emission;
    private bool _hasSpawned = false;

    public PowerupType powerupType => PowerupType.Sun;
    public bool hasSpawned => _hasSpawned;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        ps = particleSystemPrefab;
        emission = ps.emission;
        _hasSpawned = true;

        var rate = emission.rateOverTime;
        rate.constant = startRate;
        emission.rateOverTime = rate;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            onPowerupCollected?.Invoke();
            ApplyPowerup(other.GetComponent<MonoBehaviour>());
        }
    }

    public void ApplyPowerup(MonoBehaviour i)
    {
        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;

        var rate = emission.rateOverTime;
        rate.constant = boostedRate;
        emission.rateOverTime = rate;

        StartCoroutine(RevertEmissionRate());
    }

    private IEnumerator RevertEmissionRate()
    {
        yield return new WaitForSeconds(duration);

        var rate = emission.rateOverTime;
        rate.constant = startRate;
        emission.rateOverTime = rate;

        DestroyPowerup();
    }

    public void DestroyPowerup()
    {
        Destroy(gameObject);
    }
}
