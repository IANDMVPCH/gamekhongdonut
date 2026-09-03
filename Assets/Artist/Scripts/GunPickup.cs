using UnityEngine;

public class GunPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Shoot playerShoot = other.GetComponent<Shoot>();

        if (playerShoot != null)
        {
            playerShoot.hasGun = true;
            Destroy(gameObject);
        }
    }
}