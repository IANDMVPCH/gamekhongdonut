using UnityEngine;
using UnityEngine.UI;

public class Stamina: MonoBehaviour
{
    [Header("Stamina Configuration")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 25f;
    [SerializeField] private float staminaRegenRate = 15f;

    [Header("UI Reference")]
    [SerializeField] private Image staminaFillImage;

    public float CurrentStamina { get; private set; }
    public bool HasStamina => CurrentStamina > 0;

    private bool wasDrainedThisFrame;

    private void Start()
    {
        CurrentStamina = maxStamina;
        UpdateUI();
    }

    private void LateUpdate()
    {
        // Regenerate stamina immediately on frames where no stamina was used
        if (!wasDrainedThisFrame && CurrentStamina < maxStamina)
        {
            CurrentStamina += staminaRegenRate * Time.deltaTime;
            CurrentStamina = Mathf.Min(CurrentStamina, maxStamina);
            UpdateUI();
        }

        wasDrainedThisFrame = false;
    }

    public bool DrainStamina(float amountPerSecond)
    {
        if (CurrentStamina <= 0) return false;

        CurrentStamina -= amountPerSecond * Time.deltaTime;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, maxStamina);
        UpdateUI();

        wasDrainedThisFrame = true;
        return true;
    }

    public bool ConsumeStaminaOnce(float amount)
    {
        if (CurrentStamina < amount) return false;

        CurrentStamina -= amount;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, maxStamina);
        UpdateUI();

        wasDrainedThisFrame = true;
        return true;
    }

    private void UpdateUI()
    {
        if (staminaFillImage != null)
        {
            staminaFillImage.fillAmount = CurrentStamina / maxStamina;
        }
    }
}