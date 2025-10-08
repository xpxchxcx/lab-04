using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ParticleTrailLight : MonoBehaviour
{
    public ParticleSystem ps;
    public Light2D defaultLightPrefab;
    public Light2D specialLightPrefab;
    public string specialTag = "teoenming"; // Enemy tag
    public float lightDuration = 0.1f;
    public float lightIntensity = 1f;
    public float collisionRadius = 0.15f; // Radius to detect enemies
    public Color debugHitColor = Color.red;
    public Color debugMissColor = Color.green;

    private ParticleSystem.Particle[] particles;

    void Start()
    {
        if (ps == null)
            ps = GetComponent<ParticleSystem>();

        if (particles == null || particles.Length < ps.main.maxParticles)
            particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    void LateUpdate()
    {
        int count = ps.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            Vector3 worldPos = ps.transform.TransformPoint(particles[i].position);

            // Find all colliders in the radius
            Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, collisionRadius);
            Collider2D hitEnemy = null;

            // Check for a collider with the special tag
            foreach (Collider2D c in hits)
            {
                if (c.CompareTag(specialTag))
                {
                    hitEnemy = c;
                    break;
                }
            }

            Light2D lightInstance;

            if (hitEnemy != null)
            {
                // Spawn special light
                lightInstance = Instantiate(specialLightPrefab, worldPos, Quaternion.identity);
                lightInstance.intensity = lightIntensity;

                Debug.Log($"Particle hit enemy '{hitEnemy.name}'. Using SPECIAL light at {worldPos}.");
                DebugDrawCircle(worldPos, collisionRadius, debugHitColor);
            }
            else
            {
                // Spawn default light
                lightInstance = Instantiate(defaultLightPrefab, worldPos, Quaternion.identity);
                lightInstance.intensity = lightIntensity;

                Debug.Log($"• Particle at {worldPos} hit nothing. Using DEFAULT light.");
                DebugDrawCircle(worldPos, collisionRadius, debugMissColor);
            }

            Destroy(lightInstance.gameObject, lightDuration);
        }
    }

    // Draw debug circle in Scene view
    void DebugDrawCircle(Vector3 center, float radius, Color color)
    {
        int segments = 12;
        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * Mathf.PI * 2 / segments;
            float angle2 = (i + 1) * Mathf.PI * 2 / segments;
            Vector3 p1 = center + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0) * radius;
            Vector3 p2 = center + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0) * radius;
            Debug.DrawLine(p1, p2, color, 0.1f);
        }
    }
}
