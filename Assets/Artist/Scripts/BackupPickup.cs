using UnityEngine;

public class BackupModulePickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [Tooltip("Number of backup modules given to the player on pickup.")]
    public int amountToGive = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object colliding has the HealthPlayer script (or Player tag)
        if (other.GetComponent<HealthPlayer>() != null || other.CompareTag("Player"))
        {
            // Increase global backup module count
            GameData.backupModules += amountToGive;

            Debug.Log($"Backup Module collected! Total modules: {GameData.backupModules}");

            // Destroy pickup object
            Destroy(gameObject);
        }
    }
}