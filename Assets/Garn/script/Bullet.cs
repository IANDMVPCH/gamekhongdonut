using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 3f;
    public int damage = 1;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // =========================
        // NORMAL ENEMY
        // =========================

        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }


        // =========================
        // FINAL BOSS
        // =========================

        FinalBoss boss = other.GetComponent<FinalBoss>();

        if (boss != null)
        {
            boss.TakeDamage(damage);

            Debug.Log("Bullet hit FINAL BOSS for " + damage + " damage!");

            Destroy(gameObject);
            return;
        }
    }
}