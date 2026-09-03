using System.Collections;
using UnityEngine;
using TMPro; // Standard for TextMeshPro UI

public class Shoot : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Bullet Settings")]
    public float bulletSpeed = 10f;
    public float fireRate = 0.2f;

    [Header("Ammo Settings")]
    public int maxMagazineAmmo = 12;
    public int maxReserveAmmo = 90;
    public float reloadTime = 1.5f;

    [Header("UI References")]
    public TextMeshProUGUI ammoText;

    private int currentMagazineAmmo;
    private int currentReserveAmmo;
    private bool isReloading = false;
    private float nextFireTime = 0f;

    void Start()
    {
        currentMagazineAmmo = maxMagazineAmmo;
        currentReserveAmmo = maxReserveAmmo;
    }

    void Update()
    {
        UpdateUI();

        if (isReloading) return;

        // Manual Reload Input
        if (Input.GetKeyDown(KeyCode.R) && currentMagazineAmmo < maxMagazineAmmo && currentReserveAmmo > 0)
        {
            StartCoroutine(ReloadRoutine());
            return;
        }

        // Shooting Input
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            if (currentMagazineAmmo > 0)
            {
                ShootBullet();
                nextFireTime = Time.time + fireRate;
            }
            else if (currentReserveAmmo > 0)
            {
                StartCoroutine(ReloadRoutine());
            }
        }
    }

    void ShootBullet()
    {
        currentMagazineAmmo--;

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

    IEnumerator ReloadRoutine()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = maxMagazineAmmo - currentMagazineAmmo;
        int ammoToDeduct = Mathf.Min(ammoNeeded, currentReserveAmmo);

        currentMagazineAmmo += ammoToDeduct;
        currentReserveAmmo -= ammoToDeduct;

        isReloading = false;
    }

    void UpdateUI()
    {
        if (ammoText == null) return;

        if (isReloading)
        {
            ammoText.text = "RELOADIN'...";
        }
        else
        {
            ammoText.text = $"{currentMagazineAmmo} / {currentReserveAmmo}";
        }
    }

    public void AddReserveAmmo(int amount)
    {
        currentReserveAmmo = Mathf.Clamp(currentReserveAmmo + amount, 0, maxReserveAmmo);
    }
}