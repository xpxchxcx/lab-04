using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HorrorEnemy : MonoBehaviour
{
    [Header("Audio References")]
    [SerializeField] private AudioManagerSO audioManagerSO;
    [SerializeField] private AudioManagerRuntime audioManagerRuntime;
    [SerializeField] private AudioSource enemyAudio;

    [Header("Events")]
    public UnityEvent onEnemySpotted;
    public UnityEvent onEnemyLost;

    [Header("Movement Settings")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3f;
    public float rotationSpeed = 5f;

    [Header("Detection Settings")]
    public float sightRange = 5f;
    public float fieldOfView = 120f;
    public LayerMask obstacleMask;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    private Rigidbody2D rb;
    private Transform player;

    private enum State { Patrol, Chase, Investigate }
    private State currentState = State.Patrol;

    private bool playerInSight = false;
    private Vector2 lastKnownPlayerPos;
    private Vector2 investigateTarget;
    private float investigateTimer = 0f;

    public float investigateDuration = 3f;
    public float investigateRadius = 1.5f;

    private bool hasActivatedAudio = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Setup 3D audio for spatial realism
        if (enemyAudio == null)
        {
            enemyAudio = gameObject.AddComponent<AudioSource>();
            enemyAudio.spatialBlend = 1f;
            enemyAudio.minDistance = 1f;
            enemyAudio.maxDistance = 12f;
            enemyAudio.rolloffMode = AudioRolloffMode.Logarithmic;
        }
    }

    void Update()
    {
        SensePlayer();
        UpdateState();
        UpdateAudio();
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase: Chase(); break;
            case State.Investigate: Investigate(); break;
        }
    }

    // -------------------------
    // DETECTION & STATE LOGIC
    // -------------------------
    void SensePlayer()
    {
        Vector2 directionToPlayer = player.position - transform.position;
        float distance = directionToPlayer.magnitude;

        if (distance <= sightRange)
        {
            float angle = Vector2.Angle(transform.up, directionToPlayer);
            Debug.DrawRay(transform.position, directionToPlayer.normalized * sightRange, Color.blue);

            if (angle < fieldOfView / 2)
            {
                RaycastHit2D hit = Physics2D.Raycast(
                    transform.position,
                    directionToPlayer,
                    sightRange,
                    obstacleMask | (1 << LayerMask.NameToLayer("Player"))
                );

                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    playerInSight = true;
                    onEnemySpotted?.Invoke();
                    return;
                }
            }
        }

        if (playerInSight)
        {
            onEnemyLost?.Invoke();
        }

        playerInSight = false;
    }

    void UpdateState()
    {
        if (playerInSight)
        {
            lastKnownPlayerPos = player.position;
            currentState = State.Chase;
        }
        else
        {
            switch (currentState)
            {
                case State.Chase:
                    currentState = State.Investigate;
                    break;

                case State.Investigate:
                    if (Vector2.Distance(transform.position, lastKnownPlayerPos) < 0.2f)
                        currentState = State.Patrol;
                    break;

                default:
                    currentState = State.Patrol;
                    break;
            }
        }
    }

    // -------------------------
    // MOVEMENT
    // -------------------------
    void Patrol()
    {
        if (patrolPoints.Length == 0) return;
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        MoveTowards(targetPoint.position, patrolSpeed);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void Chase() => MoveTowards(player.position, chaseSpeed);

    void Investigate()
    {
        if (investigateTimer <= 0f)
        {
            investigateTarget = lastKnownPlayerPos + Random.insideUnitCircle * investigateRadius;
            investigateTimer = investigateDuration;
        }

        MoveTowards(investigateTarget, patrolSpeed);
        investigateTimer -= Time.fixedDeltaTime;

        if (Vector2.Distance(transform.position, investigateTarget) < 0.2f)
            investigateTarget = lastKnownPlayerPos + Random.insideUnitCircle * investigateRadius;

        if (investigateTimer <= 0f)
            currentState = State.Patrol;
    }

    void MoveTowards(Vector2 target, float speed)
    {
        Vector2 moveDir = (target - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);

        if (moveDir != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = Mathf.LerpAngle(rb.rotation, angle, Time.fixedDeltaTime * rotationSpeed);
        }
    }

    // -------------------------
    // AUDIO
    // -------------------------
    void UpdateAudio()
    {
        if (!audioManagerSO || !audioManagerRuntime) return;

        // Step 1: First detection of player
        if (!hasActivatedAudio && playerInSight)
        {
            hasActivatedAudio = true;
            audioManagerSO.PlayBGM(audioManagerSO.horrorTheme, true);
            audioManagerSO.FadeAudio(audioManagerRuntime.bgmSource, 1f, 1.5f);
            audioManagerSO.PlayEnemyLoop(enemyAudio, audioManagerSO.chaseClips, 1f, 3f);
            return;
        }

        if (!hasActivatedAudio) return;

        // Step 2: State-based SFX
        switch (currentState)
        {
            case State.Chase:
                audioManagerSO.PlayEnemyLoop(enemyAudio, audioManagerSO.chaseClips, 1f, 3f);
                break;

            case State.Investigate:
                audioManagerSO.PlayEnemyLoop(enemyAudio, audioManagerSO.investigateClips, 2f, 5f);
                break;

            case State.Patrol:
                audioManagerSO.PlayEnemyLoop(enemyAudio, audioManagerSO.patrolClips, 3f, 6f);
                break;
        }

        // Step 3: Dynamic volume by distance
        float dist = Vector2.Distance(transform.position, player.position);
        float normalized = Mathf.InverseLerp(10f, 1f, dist);
        enemyAudio.volume = Mathf.Lerp(0.2f, 1f, normalized);
    }

    // -------------------------
    // DEBUG VISUALS
    // -------------------------
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 forward = transform.up * sightRange;
        Quaternion leftRay = Quaternion.Euler(0, 0, -fieldOfView / 2);
        Quaternion rightRay = Quaternion.Euler(0, 0, fieldOfView / 2);
        Gizmos.DrawRay(transform.position, leftRay * forward);
        Gizmos.DrawRay(transform.position, rightRay * forward);
        Gizmos.DrawWireSphere(transform.position, sightRange);

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.1f);

                int next = (i + 1) % patrolPoints.Length;
                if (patrolPoints[next] != null)
                    Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[next].position);
            }
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(lastKnownPlayerPos, 0.15f);
    }
}
