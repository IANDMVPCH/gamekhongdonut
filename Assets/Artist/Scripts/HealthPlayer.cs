using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthPlayer : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;

    [Header("UI Settings")]
    public Image healthFill;
    public float smoothSpeed = 5f;

    [Header("Death & Respawn")]
    public string deathSceneName = "GameOver";
    public string backupSceneName = "BackupScene";

    private int currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        // Initialize health bar instantly at full on start
        if (healthFill != null)
        {
            healthFill.fillAmount = 1f;
        }
    }

    void Update()
    {
        // Smoothly update health bar UI
        UpdateHealthBarUI();

        // TEST: H = damage
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }

        // TEST: J = heal
        if (Input.GetKeyDown(KeyCode.J))
        {
            Heal(10);
        }
    }

    void UpdateHealthBarUI()
    {
        if (healthFill == null) return;

        float targetFill = (float)currentHealth / maxHealth;

        // Smoothly move the bar toward the target fill amount
        healthFill.fillAmount = Mathf.Lerp(
            healthFill.fillAmount,
            targetFill,
            smoothSpeed * Time.deltaTime
        );
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead)
            return;

        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log("Player Health: " + currentHealth);
    }

    public void SetHealth(int health)
    {
        if (isDead)
            return;

        currentHealth = Mathf.Clamp(health, 0, maxHealth);

        Debug.Log("Health set to: " + currentHealth);
    }

    void Die()
    {
        isDead = true;

        Debug.Log("Player Died!");

        // Check global backup module counter
        if (GameData.backupModules > 0)
        {
            GameData.backupModules--; // Consume one backup module
            Debug.Log($"Using backup module! Modules remaining: {GameData.backupModules}");
            SceneManager.LoadScene(backupSceneName);
        }
        else
        {
            Debug.Log("No backup modules left. Loading Game Over.");
            SceneManager.LoadScene(deathSceneName);
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}