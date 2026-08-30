using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public GameObject bullet;
    public float bulletSpeed = 5f;

    float timer = 0f;
    bool canShoot = true;
    public float shootCooldown = 0.5f;

    private void HandleCooldown()
    {
        if (!canShoot)
        {
            timer += Time.deltaTime;
            if (timer >= shootCooldown)
            {
                timer = 0f;
                canShoot = true;
            }
        }
    }

    void Shoot()
    {
        if (canShoot)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            GameObject inst = GameObject.Instantiate(bullet, transform.position + 1f * dir, Quaternion.identity);
            inst.GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;
            canShoot = false;
        }
    }

    void Update()
    {
        HandleCooldown();
        Shoot();
    }
}
