using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorrorEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3f;
    public float rotationSpeed = 5f;

    [Header("Detection Settings")]
    public float sightRange = 5f;
    public float fieldOfView = 120f; // degrees
    public LayerMask obstacleMask; // assign "Walls" layer in inspector

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
    public float investigateDuration = 3f; // seconds
    public float investigateRadius = 1.5f; // how far it wanders around last known pos



    private bool hasActivatedAudio = false;
    private Coroutine currentSFXRoutine;


    private AudioSource enemyAudio;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; // Kinematic Rigidbody
        player = GameObject.FindGameObjectWithTag("Player").transform;

        enemyAudio = GetComponent<AudioSource>();
        if (enemyAudio == null)
        {
            enemyAudio = gameObject.AddComponent<AudioSource>();
            enemyAudio.spatialBlend = 1f; // 3D sound
            enemyAudio.minDistance = 1f;
            enemyAudio.maxDistance = 12f;
            enemyAudio.rolloffMode = AudioRolloffMode.Logarithmic;
        }
    }

    void Update()
    {
        UpdateAudio();
        SensePlayer();

        // Update state
        if (playerInSight)
        {
            lastKnownPlayerPos = player.position; // save last seen position
            currentState = State.Chase;

        }
        else
        {
            // If we lost the player while chasing, go to Investigate
            if (currentState == State.Chase)
                currentState = State.Investigate;
            else if (currentState == State.Investigate)
            {
                // Stay in Investigate until reaching last known pos
                if (Vector2.Distance(transform.position, lastKnownPlayerPos) < 0.2f)
                    currentState = State.Patrol;
            }
            else
            {
                currentState = State.Patrol;


            }
        }

        Debug.Log($"State: {currentState}, PlayerInSight: {playerInSight}");
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
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, sightRange, obstacleMask | (1 << LayerMask.NameToLayer("Player")));
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    playerInSight = true;
                    return;
                }
            }
        }

        playerInSight = false;
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        MoveTowards(targetPoint.position, patrolSpeed);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    void Chase()
    {
        MoveTowards(player.position, chaseSpeed);
    }

    void Investigate()
    {
        if (investigateTimer <= 0f)
        {
            // Start investigation
            investigateTarget = lastKnownPlayerPos + Random.insideUnitCircle * investigateRadius;
            investigateTimer = investigateDuration;
        }

        MoveTowards(investigateTarget, patrolSpeed);
        investigateTimer -= Time.fixedDeltaTime;

        // If reached current target, pick a new nearby point
        if (Vector2.Distance(transform.position, investigateTarget) < 0.2f)
        {
            investigateTarget = lastKnownPlayerPos + Random.insideUnitCircle * investigateRadius;
        }

        // After investigation time ends, go back to patrol
        if (investigateTimer <= 0f)
        {
            currentState = State.Patrol;
        }
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

    // -------- GIZMOS --------
    void OnDrawGizmosSelected()
    {
        // Sight cone
        Gizmos.color = Color.red;
        Vector3 forward = transform.up * sightRange;
        Quaternion leftRayRotation = Quaternion.Euler(0, 0, -fieldOfView / 2);
        Quaternion rightRayRotation = Quaternion.Euler(0, 0, fieldOfView / 2);
        Gizmos.DrawRay(transform.position, leftRayRotation * forward);
        Gizmos.DrawRay(transform.position, rightRayRotation * forward);
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // Patrol path
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

        // Last known player position
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(lastKnownPlayerPos, 0.15f);
    }
    private IEnumerator DelayedBGMStart()
    {
        yield return new WaitForSeconds(1.5f);
        AudioManager.I.PlayBGM(AudioManager.I.horrorTheme, true);
        AudioManager.I.FadeAudio(AudioManager.I.GetBGMSource(), 1f, 1.5f);
    }

    void UpdateAudio()
    {


        // Don’t play any sounds until enemy first sees the player
        if (!hasActivatedAudio && playerInSight)
        {
            hasActivatedAudio = true;

            // Switch to horror BGM once
            AudioManager.I.PlayBGM(AudioManager.I.horrorTheme, true);
            AudioManager.I.FadeAudio(AudioManager.I.GetBGMSource(), 1f, 1.5f);

            // Begin chase sounds
            AudioManager.I.PlayEnemyChaseLoop(enemyAudio);

            return;
        }

        if (!hasActivatedAudio) return;

        // State-based SFX
        if (playerInSight)
            AudioManager.I.PlayEnemyChaseLoop(enemyAudio);
        else if (currentState == State.Investigate)
            AudioManager.I.PlayEnemyInvestigateLoop(enemyAudio);
        else if (currentState == State.Patrol)
            AudioManager.I.PlayEnemyPatrolLoop(enemyAudio);

        // Optional: dynamically adjust volume based on player distance
        float dist = Vector2.Distance(transform.position, player.position);
        float normalized = Mathf.InverseLerp(10f, 1f, dist); // closer = louder
        float volume = Mathf.Lerp(0.2f, 1f, normalized);
        AudioManager.I.SetSFXVolume(volume);

        // Print what SFX volume AudioManager is using
        Debug.Log($"AudioManager SFX Volume: {volume}");
    }

    void SwitchSFXLoop(AudioClip[] clips, float minInterval, float maxInterval)
    {
        // if already playing this type of loop, do nothing
        if (AudioManager.I.CurrentClipArray == clips) return;

        // stop old loop
        if (currentSFXRoutine != null)
        {
            StopCoroutine(currentSFXRoutine);
        }

        // remember which clip set is active
        AudioManager.I.CurrentClipArray = clips;

        // start new loop
        currentSFXRoutine = StartCoroutine(AudioManager.I.PlayRandomSFXCoroutine(clips, minInterval, maxInterval));
    }
}
