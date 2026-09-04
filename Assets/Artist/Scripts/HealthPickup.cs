using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("Amount of health restored on pickup.")]
    public int healthToRestore = 25;

    private void OnTriggerEnter2D(Collider2D other)
    {
        HealthPlayer playerHealth = other.GetComponent<HealthPlayer>();

        if (playerHealth != null && playerHealth.GetCurrentHealth() < playerHealth.GetMaxHealth())
        {
            playerHealth.Heal(healthToRestore);
            Destroy(gameObject);
        }
    }
}