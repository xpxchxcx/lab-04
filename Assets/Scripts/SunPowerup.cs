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
        TopDownBatController batController;
        bool result = i.TryGetComponent<TopDownBatController>(out batController);
        if (result)
        {
            batController.HasSunPowerup = true;
        }

        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;

        // FSM style powerup application
        BatStateController bat;
        bool r = i.TryGetComponent<BatStateController>(out bat);

        if (r)
        {
            bat.AddPowerup(this.powerupType);
        }
    }


    public void DestroyPowerup()
    {
        Destroy(gameObject);
    }
}
