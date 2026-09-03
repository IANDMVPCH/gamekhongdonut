using System.Collections;
using UnityEngine;
using TMPro;

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

    [Header("Gun Ownership & Visuals")]
    public bool hasGun = false;
    public SpriteRenderer gunSprite; // Drag gun object's SpriteRenderer here

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
        // Toggle gun sprite visibility
        if (gunSprite != null)
        {
            gunSprite.enabled = hasGun;
        }

        // If player doesn't have a gun, hide UI and block input
        if (!hasGun)
        {
            if (ammoText != null) ammoText.text = "";
            return;
        }

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

        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = -Camera.main.transform.position.z;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        Vector2 direction = ((Vector2)mouseWorldPosition - (Vector2)firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = direction * bulletSpeed;
        }

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