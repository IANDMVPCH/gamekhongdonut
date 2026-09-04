using UnityEngine;

public class BossRock : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 50;

    [Header("Lifetime")]
    public float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HealthPlayer player = other.GetComponent<HealthPlayer>();

        if (player != null)
        {
            player.TakeDamage(damage);

            Debug.Log("Boss rock hit player for " + damage + " damage!");

            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HealthPlayer player = collision.gameObject.GetComponent<HealthPlayer>();

        if (player != null)
        {
            player.TakeDamage(damage);

            Debug.Log("Boss rock hit player for " + damage + " damage!");

            Destroy(gameObject);
        }
    }
}