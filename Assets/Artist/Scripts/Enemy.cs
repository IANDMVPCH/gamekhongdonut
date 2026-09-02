using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    public enum AIState { Idle, TargetSpotted, Searching }

    [Header("State")]
    [SerializeField] private AIState currentState = AIState.Idle;

    [Header("Target & Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float sightRadius = 10f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Combat Settings")]
    [SerializeField] private float idealRange = 4f;
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float projectileSpeed = 12f;

    [Header("Movement & Search")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float searchDuration = 5f;
    [SerializeField] private float searchAreaRadius = 3f;

    [Header("Obstacle Avoidance")]
    [SerializeField] private float enemyRadius = 0.4f;      // Matches enemy collider size
    [SerializeField] private float lookAheadDistance = 1.5f; // Sensing distance ahead
    [SerializeField] private int rayCount = 16;              // Number of 360-degree detection rays

    private Vector3 lastKnownPosition;
    private Vector3 currentSearchWaypoint;
    private bool reachedLastKnownPos;
    private float nextFireTime;
    private float searchTimer;
    private Rigidbody2D rb;
    private Collider2D myCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        
        rb.gravityScale = 0f;

        // Prevent physical friction lockups against walls
        PhysicsMaterial2D frictionless = new PhysicsMaterial2D("Frictionless") { friction = 0f };
        rb.sharedMaterial = frictionless;

        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        bool canSeePlayer = CheckLineOfSight();

        if (canSeePlayer)
        {
            lastKnownPosition = player.position;
            currentState = AIState.TargetSpotted;
        }
        else if (currentState == AIState.TargetSpotted)
        {
            currentState = AIState.Searching;
            searchTimer = searchDuration;
            reachedLastKnownPos = false;
        }

        ExecuteState();
    }

    private bool CheckLineOfSight()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > sightRadius) return false;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        myCollider.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask);
        myCollider.enabled = true;

        return hit.collider == null;
    }

    private void ExecuteState()
    {
        switch (currentState)
        {
            case AIState.Idle:
                rb.velocity = Vector2.zero;
                break;

            case AIState.TargetSpotted:
                TryShoot();

                float distanceToPlayer = Vector2.Distance(transform.position, player.position);

                if (distanceToPlayer > idealRange)
                {
                    MoveTowards(player.position);
                }
                else
                {
                    rb.velocity = Vector2.zero;
                }
                break;

            case AIState.Searching:
                searchTimer -= Time.deltaTime;

                if (searchTimer <= 0f)
                {
                    currentState = AIState.Idle;
                    rb.velocity = Vector2.zero;
                    break;
                }

                if (!reachedLastKnownPos)
                {
                    MoveTowards(lastKnownPosition);

                    if (Vector2.Distance(transform.position, lastKnownPosition) <= 0.5f)
                    {
                        reachedLastKnownPos = true;
                        GenerateNextSearchWaypoint();
                    }
                }
                else
                {
                    MoveTowards(currentSearchWaypoint);

                    if (Vector2.Distance(transform.position, currentSearchWaypoint) <= 0.5f)
                    {
                        GenerateNextSearchWaypoint();
                    }
                }
                break;
        }
    }

    private void GenerateNextSearchWaypoint()
    {
        Vector2 randomOffset = Random.insideUnitCircle * searchAreaRadius;
        currentSearchWaypoint = lastKnownPosition + (Vector3)randomOffset;
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector2 lineOfSightVector = ((Vector2)targetPosition - rb.position).normalized;
        Vector2 finalMoveDirection = GetContextSteeringDirection(targetPosition, lineOfSightVector);

        // Smoothly set velocity along the chosen clearance direction
        rb.velocity = finalMoveDirection * moveSpeed;
    }

    // Context-Based Steering: Samples 360-degree directions to round box corners cleanly
    private Vector2 GetContextSteeringDirection(Vector3 targetPosition, Vector2 lineOfSightVector)
    {
        myCollider.enabled = false;

        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
        float checkDistance = Mathf.Min(distanceToTarget, lookAheadDistance);

        // Direct path check
        RaycastHit2D directHit = Physics2D.CircleCast(transform.position, enemyRadius, lineOfSightVector, checkDistance, obstacleMask);
        if (directHit.collider == null)
        {
            myCollider.enabled = true;
            return lineOfSightVector;
        }

        Vector2 bestDirection = lineOfSightVector;
        float highestScore = float.NegativeInfinity;

        // Evaluate radial ray directions around the AI
        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * (360f / rayCount);
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;

            // Interest: Preference for directions pointing toward target (-1 to 1)
            float interest = Vector2.Dot(dir, lineOfSightVector);

            // Danger: Heavy penalty for directions hitting box faces or corners
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, enemyRadius, dir, lookAheadDistance, obstacleMask);
            float danger = 0f;

            if (hit.collider != null)
            {
                float normalizedDist = hit.distance / lookAheadDistance;
                danger = (1f - normalizedDist) * 3f; // Proximity multiplier
            }

            float score = interest - danger;

            if (score > highestScore)
            {
                highestScore = score;
                bestDirection = dir;
            }
        }

        myCollider.enabled = true;
        return bestDirection;
    }

    private void TryShoot()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / fireRate);
            Shoot();
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPosition = transform.position + (Vector3)offset;
        Vector2 shootDirection = (player.position - spawnPosition).normalized;
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();
        if (projRb != null)
        {
            projRb.velocity = shootDirection * projectileSpeed;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, idealRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + (Vector3)offset, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, enemyRadius);

        if (currentState == AIState.Searching)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(lastKnownPosition, searchAreaRadius);
            Gizmos.DrawSphere(currentSearchWaypoint, 0.2f);
        }
    }
}