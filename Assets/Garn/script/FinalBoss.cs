using System.Collections;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    // =========================================================
    // PLAYER
    // =========================================================

    [Header("Player")]
    public Transform player;


    // =========================================================
    // BOSS HEALTH
    // =========================================================

    [Header("Boss Health")]
    public int maxHealth = 500;

    private int currentHealth;
    private bool isDead = false;


    // =========================================================
    // ROCK ATTACK
    // =========================================================

    [Header("Rock Attack")]
    public GameObject rockPrefab;
    public Transform rockSpawnPoint;

    public float rockSpeed = 8f;
    public float rockCooldown = 3f;

    private float nextRockTime;


    // =========================================================
    // COSMIC ZONE ATTACK
    // =========================================================

    [Header("Cosmic Zone")]
    public GameObject cosmicZonePrefab;

    public float cosmicZoneCooldown = 8f;
    public float cosmicZoneDelay = 1.5f;

    private float nextZoneTime;

    private bool usingZone = false;


    // =========================================================
    // VICTORY PORTAL
    // =========================================================

    [Header("Victory Portal")]
    public GameObject portalPrefab;
    public Transform portalSpawnPoint;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // Set boss health
        currentHealth = maxHealth;


        // Find player automatically
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }


        // Delay the first attacks
        nextRockTime = Time.time + 2f;
        nextZoneTime = Time.time + 5f;


        Debug.Log(
            "Final Boss spawned with " +
            currentHealth +
            " HP."
        );
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (isDead)
            return;

        if (player == null)
            return;


        RockAttack();
        CosmicZoneAttack();
    }


    // =========================================================
    // ROCK ATTACK
    // =========================================================

    private void RockAttack()
    {
        if (Time.time < nextRockTime)
            return;

        // Don't throw rocks while preparing Cosmic Zone
        if (usingZone)
            return;


        nextRockTime =
            Time.time + rockCooldown;


        ThrowRock();
    }


    private void ThrowRock()
    {
        if (rockPrefab == null)
        {
            Debug.LogWarning(
                "Final Boss: Rock Prefab is missing!"
            );

            return;
        }


        // Get spawn position
        Vector3 spawnPosition =
            transform.position;

        if (rockSpawnPoint != null)
        {
            spawnPosition =
                rockSpawnPoint.position;
        }


        // Aim toward player
        Vector2 direction =
            ((Vector2)player.position -
             (Vector2)spawnPosition).normalized;


        // Spawn rock
        GameObject rock =
            Instantiate(
                rockPrefab,
                spawnPosition,
                Quaternion.identity
            );


        // Give rock velocity
        Rigidbody2D rockRb =
            rock.GetComponent<Rigidbody2D>();

        if (rockRb != null)
        {
            rockRb.linearVelocity =
                direction * rockSpeed;
        }


        // Rotate rock toward player
        float angle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) * Mathf.Rad2Deg;

        rock.transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                angle
            );
    }


    // =========================================================
    // COSMIC ZONE ATTACK
    // =========================================================

    private void CosmicZoneAttack()
    {
        if (Time.time < nextZoneTime)
            return;

        if (usingZone)
            return;


        nextZoneTime =
            Time.time + cosmicZoneCooldown;


        StartCoroutine(
            SpawnCosmicZone()
        );
    }


    private IEnumerator SpawnCosmicZone()
    {
        usingZone = true;


        // Remember where the player is
        // when the boss starts the attack
        Vector3 targetPosition =
            player.position;


        Debug.Log(
            "Boss is preparing Cosmic Zone!"
        );


        // Warning time
        yield return new WaitForSeconds(
            cosmicZoneDelay
        );


        // Don't spawn the zone if the boss died
        if (isDead)
        {
            usingZone = false;
            yield break;
        }


        // Spawn Cosmic Zone
        if (cosmicZonePrefab != null)
        {
            Instantiate(
                cosmicZonePrefab,
                targetPosition,
                Quaternion.identity
            );

            Debug.Log(
                "Cosmic Zone spawned!"
            );
        }
        else
        {
            Debug.LogWarning(
                "Final Boss: Cosmic Zone Prefab is missing!"
            );
        }


        usingZone = false;
    }


    // =========================================================
    // BOSS DAMAGE
    // =========================================================

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;


        currentHealth -= damage;


        // Prevent negative HP
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }


        Debug.Log(
            "Boss Health: " +
            currentHealth +
            " / " +
            maxHealth
        );


        // Check if boss is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }


    // =========================================================
    // BOSS DEATH
    // =========================================================

    private void Die()
    {
        if (isDead)
            return;


        isDead = true;


        Debug.Log(
            "FINAL BOSS DEFEATED!"
        );


        // Stop Cosmic Zone preparation
        StopAllCoroutines();


        // =====================================================
        // SPAWN VICTORY PORTAL
        // =====================================================

        if (portalPrefab != null)
        {
            Vector3 spawnPosition;


            // Use the middle-of-map spawn point
            if (portalSpawnPoint != null)
            {
                spawnPosition =
                    portalSpawnPoint.position;
            }
            else
            {
                // Fallback:
                // spawn where the boss died
                spawnPosition =
                    transform.position;
            }


            Instantiate(
                portalPrefab,
                spawnPosition,
                Quaternion.identity
            );


            Debug.Log(
                "Victory portal appeared!"
            );
        }
        else
        {
            Debug.LogWarning(
                "Final Boss: Portal Prefab is missing!"
            );
        }


        // Destroy boss
        Destroy(gameObject);
    }


    // =========================================================
    // HEALTH GETTERS
    // =========================================================

    public int GetCurrentHealth()
    {
        return currentHealth;
    }


    public int GetMaxHealth()
    {
        return maxHealth;
    }
}