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

    [Header("Friendly Fire Avoidance")]
    [SerializeField] private LayerMask allyMask;             // Layer assigned to Enemy GameObjects
    [SerializeField] private float friendlyFireRadius = 0.3f; // Width of the friendly fire safety check

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

    private bool IsAllyInLineOfFire()
    {
        Vector3 spawnPosition = transform.position + (Vector3)offset;
        Vector2 directionToPlayer = (player.position - spawnPosition).normalized;
        float distanceToPlayer = Vector2.Distance(spawnPosition, player.position);

        myCollider.enabled = false;
        // CircleCast towards player to detect ally bodies in the shot window
        RaycastHit2D hit = Physics2D.CircleCast(spawnPosition, friendlyFireRadius, directionToPlayer, distanceToPlayer, allyMask);
        myCollider.enabled = true;

        return hit.collider != null && hit.collider.gameObject != gameObject;
    }

    private void ExecuteState()
    {
        switch (currentState)
        {
            case AIState.Idle:
                rb.velocity = Vector2.zero;
                break;

            case AIState.TargetSpotted:
                bool lineOfFireBlocked = IsAllyInLineOfFire();

                // Only fire if an ally is not in the line of fire
                if (!lineOfFireBlocked)
                {
                    TryShoot();
                }

                float distanceToPlayer = Vector2.Distance(transform.position, player.position);

                // Reposition if out of range OR if shot is blocked by an ally
                if (distanceToPlayer > idealRange || lineOfFireBlocked)
                {
                    MoveTowards(player.position, lineOfFireBlocked);
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
                    MoveTowards(lastKnownPosition, false);

                    if (Vector2.Distance(transform.position, lastKnownPosition) <= 0.5f)
                    {
                        reachedLastKnownPos = true;
                        GenerateNextSearchWaypoint();
                    }
                }
                else
                {
                    MoveTowards(currentSearchWaypoint, false);

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

    private void MoveTowards(Vector3 targetPosition, bool strafeMode)
    {
        Vector2 lineOfSightVector = ((Vector2)targetPosition - rb.position).normalized;
        Vector2 finalMoveDirection = GetContextSteeringDirection(targetPosition, lineOfSightVector, strafeMode);

        rb.velocity = finalMoveDirection * moveSpeed;
    }

    // Context-Based Steering: Includes ally collision and strafe scoring when shot is blocked
    private Vector2 GetContextSteeringDirection(Vector3 targetPosition, Vector2 lineOfSightVector, bool strafeMode)
    {
        myCollider.enabled = false;

        LayerMask combinedMask = obstacleMask | allyMask;

        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
        float checkDistance = Mathf.Min(distanceToTarget, lookAheadDistance);

        // Direct path check (including allies)
        if (!strafeMode)
        {
            RaycastHit2D directHit = Physics2D.CircleCast(transform.position, enemyRadius, lineOfSightVector, checkDistance, combinedMask);
            if (directHit.collider == null || directHit.collider.gameObject == gameObject)
            {
                myCollider.enabled = true;
                return lineOfSightVector;
            }
        }

        Vector2 bestDirection = lineOfSightVector;
        float highestScore = float.NegativeInfinity;

        // Evaluate radial ray directions around the AI
        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * (360f / rayCount);
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;

            // Interest: Standard forward bias, or lateral bias during strafe mode
            float interest = Vector2.Dot(dir, lineOfSightVector);
            if (strafeMode)
            {
                // Penalize forward/backward movement and reward perpendicular angles to flank
                interest = 1f - Mathf.Abs(interest);
            }

            // Danger: Heavy penalty for obstacles and allies
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, enemyRadius, dir, lookAheadDistance, combinedMask);
            float danger = 0f;

            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                float normalizedDist = hit.distance / lookAheadDistance;
                danger = (1f - normalizedDist) * 3f;
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
        Gizmos.DrawSphere(transform.position + (Vector3)offset, friendlyFireRadius);

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