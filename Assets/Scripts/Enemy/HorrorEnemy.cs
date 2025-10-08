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

    private enum State { Patrol, Chase }
    private State currentState = State.Patrol;

    private bool playerInSight = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; // Kinematic Rigidbody
        player = GameObject.FindGameObjectWithTag("Player").transform; // your bat should have tag "Player"
    }

    void Update()
    {
        SensePlayer();

        // Update state
        if (playerInSight)
            currentState = State.Chase;
        else
            currentState = State.Patrol;

        Debug.Log($"State: {currentState}, PlayerInSight: {playerInSight}");
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase: Chase(); break;
        }
    }

    void SensePlayer()
    {
        Vector2 directionToPlayer = player.position - transform.position;
        float distance = directionToPlayer.magnitude;

        // Check line of sight
        if (distance <= sightRange)
        {
            float angle = Vector2.Angle(transform.up, directionToPlayer);
            Debug.DrawRay(transform.position, directionToPlayer.normalized * sightRange, Color.blue);

            if (angle < fieldOfView / 2)
            {
                // Raycast to check if wall is blocking
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
    }
}
