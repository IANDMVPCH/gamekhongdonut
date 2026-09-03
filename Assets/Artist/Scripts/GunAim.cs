using UnityEngine;

public class GunAim : MonoBehaviour
{
    private Camera mainCam;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        mainCam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 1. Get mouse position in screen space and convert to 2D world space
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = -mainCam.transform.position.z;
        Vector3 mouseWorldPosition = mainCam.ScreenToWorldPoint(mouseScreenPosition);

        // 2. Calculate direction from gun to mouse
        Vector2 direction = ((Vector2)mouseWorldPosition - (Vector2)transform.position).normalized;

        // 3. Calculate rotation angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 4. Apply rotation around the Z axis
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 5. Flip sprite vertically when aiming left so the gun stays right-side up
        if (spriteRenderer != null)
        {
            if (angle > 90f || angle < -90f)
            {
                spriteRenderer.flipY = true;
            }
            else
            {
                spriteRenderer.flipY = false;
            }
        }
    }
}