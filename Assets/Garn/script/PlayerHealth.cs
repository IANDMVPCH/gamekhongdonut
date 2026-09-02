using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;

    [Header("Death")]
    public string deathSceneName = "GameOver";

    private int currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
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

        SceneManager.LoadScene(deathSceneName);
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