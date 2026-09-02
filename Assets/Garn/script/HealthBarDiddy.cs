using UnityEngine;
using UnityEngine.UI;

public class HealthBarDiddy : MonoBehaviour
{
    [Header("Health")]
    public PlayerHealth playerHealth;
    public Image healthFill;

    [Header("Smoothness")]
    public float smoothSpeed = 5f;

    private float targetHealth;

    void Start()
    {
        targetHealth = GetHealthPercent();
        healthFill.fillAmount = targetHealth;
    }

    void Update()
    {
        // Get the player's current health
        targetHealth = GetHealthPercent();

        // Smoothly move the bar toward the target
        healthFill.fillAmount = Mathf.Lerp(
            healthFill.fillAmount,
            targetHealth,
            smoothSpeed * Time.deltaTime
        );
    }

    float GetHealthPercent()
    {
        if (playerHealth == null)
            return 0f;

        return (float)playerHealth.GetCurrentHealth() /
               playerHealth.GetMaxHealth();
    }
}