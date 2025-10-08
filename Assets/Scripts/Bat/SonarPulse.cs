using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SonarPulse : MonoBehaviour
{
    private ParticleSystem ps;
    private Light pulseLight;

    [Header("Light Settings")]
    public bool useGlowLight = true;
    public float lightIntensity = 5f;
    public float lightRange = 10f;
    public Color lightColor = Color.cyan;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        // Optional glow light
        if (useGlowLight)
        {
            pulseLight = gameObject.AddComponent<Light>();
            pulseLight.type = LightType.Point;
            pulseLight.intensity = lightIntensity;
            pulseLight.range = lightRange;
            pulseLight.color = lightColor;
        }
    }

    void OnEnable()
    {
        ps.Play();
    }

    void Update()
    {
        // Fade light with particle lifetime
        if (pulseLight != null)
        {
            float lifePercent = 1f - (ps.time / ps.main.duration);
            pulseLight.intensity = Mathf.Lerp(0, lightIntensity, lifePercent);
        }

        // Destroy when particle system finishes
        if (!ps.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
