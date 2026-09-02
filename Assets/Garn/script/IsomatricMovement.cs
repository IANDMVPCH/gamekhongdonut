using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class IsometricMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.5f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isDashing;
    private float nextDashTime;
    Stamina stamina;
    [SerializeField] private float dashStamina = 50f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stamina = GetComponent<Stamina>();
    }

    void Update()
    {
        // Calculate input direction
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        movement = new Vector2(horizontal - vertical, horizontal + vertical).normalized;

        // Trigger Dash
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextDashTime && !isDashing && stamina.CurrentStamina >= dashStamina)
        {
            Dash();
            stamina.ConsumeStaminaOnce(dashStamina);
        }
    }

    void FixedUpdate()
    {
        // Skip velocity override while dashing
        if (isDashing) return;

        rb.linearVelocity = movement * moveSpeed;
    }

    void Dash()
    {
        nextDashTime = Time.time + dashCooldown;

        Vector2 dashDir;

        // Dash in input direction if holding movement keys, otherwise toward mouse cursor
        if (movement.sqrMagnitude > 0.01f)
        {
            dashDir = movement;
        }
        else
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = -Camera.main.transform.position.z;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            dashDir = ((Vector2)mouseWorldPosition - (Vector2)transform.position).normalized;
        }

        StartCoroutine(PerformDash(dashDir));
    }

    private IEnumerator PerformDash(Vector2 direction)
    {
        isDashing = true;
        rb.linearVelocity = direction * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }
}