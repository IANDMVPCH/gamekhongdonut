using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 5f;

    [Header("Collision Layers")]
    [Tooltip("Layers listed here will be ignored by the bullet (e.g., Enemy, EnemyBullet).")]
    [SerializeField] private LayerMask exceptionLayers;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        // Deal damage if the non-exception object is the player
        if (collision.CompareTag("Player"))
        {
            HealthPlayer player = collision.GetComponent<HealthPlayer>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
        //Destroy(gameObject);
    }
}