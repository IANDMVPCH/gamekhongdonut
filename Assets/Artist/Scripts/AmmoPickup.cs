using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Ammo Settings")]
    [Tooltip("Amount of reserve ammo given to the player on pickup.")]
    public int ammoToGet = 30;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object or its parent has the Shoot script
        Shoot playerShoot = other.GetComponent<Shoot>();

        if (playerShoot != null)
        {
            playerShoot.AddReserveAmmo(ammoToGet);
            Destroy(gameObject);
        }
    }
}