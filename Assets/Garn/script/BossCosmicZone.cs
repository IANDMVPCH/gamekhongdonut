using UnityEngine;

public class BossCosmicZone : MonoBehaviour
{
    [Header("Attack")]
    public int damage = 25;
    public float duration = 4f;

    [Header("Pull")]
    public float pullForce = 8f;

    [Header("Damage Rate")]
    public float damageInterval = 0.5f;

    private float nextDamageTime;

    private void Start()
    {
        Destroy(gameObject, duration);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        HealthPlayer player = other.GetComponent<HealthPlayer>();

        if (player == null)
            return;

        // =========================
        // PULL PLAYER
        // =========================

        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            Vector2 direction =
                ((Vector2)transform.position - playerRb.position).normalized;

            playerRb.AddForce(direction * pullForce);
        }

        // =========================
        // DAMAGE PLAYER
        // =========================

        if (Time.time >= nextDamageTime)
        {
            player.TakeDamage(damage);

            Debug.Log("Cosmic Zone damaged player for " + damage);

            nextDamageTime = Time.time + damageInterval;
        }
    }
}