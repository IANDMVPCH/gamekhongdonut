using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Bullet Settings")]
    public float bulletSpeed = 10f;
    public float fireRate = 0.2f;

    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
{
    // 1. Get mouse position in screen pixels
    Vector3 mouseScreenPosition = Input.mousePosition;

    // 2. Set Z depth to the distance between camera and the 2D plane (Z = 0)
    mouseScreenPosition.z = -Camera.main.transform.position.z;

    // 3. Convert screen pixels to world coordinates at the correct depth
    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

    // 4. Calculate direction from FirePoint to mouse position
    Vector2 direction = ((Vector2)mouseWorldPosition - (Vector2)firePoint.position).normalized;

    // Spawn bullet
    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

    // Set velocity
    Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
    if (bulletRb != null)
    {
        bulletRb.linearVelocity = direction * bulletSpeed;
    }

    // Rotate bullet toward cursor
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);
}
}