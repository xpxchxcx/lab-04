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
    public float collisionRadius = 0.15f; // For ray thickness
    public LayerMask environmentLayer; // assign Environment layer here
    public Color debugHitColor = Color.red;
    public Color debugMissColor = Color.green;

    private ParticleSystem.Particle[] particles;
    private Vector3[] prevPositions; // track previous positions for raycast

    void Start()
    {
        if (ps == null)
            ps = GetComponent<ParticleSystem>();

        int maxParticles = ps.main.maxParticles;
        if (particles == null || particles.Length < maxParticles)
            particles = new ParticleSystem.Particle[maxParticles];
        prevPositions = new Vector3[maxParticles];
    }

    void LateUpdate()
    {
        int count = ps.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            Vector3 worldPos = ps.transform.TransformPoint(particles[i].position);
            Vector3 prevPos = prevPositions[i];
            prevPositions[i] = worldPos; // store for next frame

            // --- always spawn default trail light like before ---
            Light2D trailLight = Instantiate(defaultLightPrefab, worldPos, Quaternion.identity);
            trailLight.intensity = lightIntensity;
            Destroy(trailLight.gameObject, lightDuration);
            DebugDrawCircle(worldPos, collisionRadius, debugMissColor);

            // skip the first frame for new particles
            if (prevPos == Vector3.zero)
                continue;

            // ray direction and distance
            Vector2 dir = worldPos - prevPos;
            float dist = dir.magnitude;
            if (dist <= 0.001f)
                continue;

            // perform raycast to detect collisions
            RaycastHit2D hit = Physics2D.CircleCast(prevPos, collisionRadius, dir.normalized, dist, environmentLayer);

            if (hit.collider != null)
            {
                Light2D lightInstance;

                // check if hit enemy
                if (hit.collider.CompareTag(specialTag))
                {
                    // special light (temporary)
                    lightInstance = Instantiate(specialLightPrefab, hit.point, Quaternion.identity);
                    lightInstance.intensity = lightIntensity;
                    Debug.Log($"Particle hit enemy '{hit.collider.name}' at {hit.point}");
                    DebugDrawCircle(hit.point, collisionRadius, debugHitColor);
                    Destroy(lightInstance.gameObject, lightDuration);
                }
                else
                {
                    // environment hit → persistent light
                    lightInstance = Instantiate(defaultLightPrefab, hit.point, Quaternion.identity);
                    lightInstance.intensity = lightIntensity;
                    Debug.Log($"Particle hit environment '{hit.collider.name}' at {hit.point}. Persistent light placed.");
                    DebugDrawCircle(hit.point, collisionRadius, debugMissColor);
                    // persistent — no destroy
                }
            }
        }
    }

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
