using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    [Header("Stamina Configuration")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 25f;
    [SerializeField] private float staminaRegenRate = 15f;

    [Header("UI Reference")]
    [SerializeField] private Image staminaFillImage;

    [Header("Bar Smoothness")]
    [SerializeField] private float smoothSpeed = 8f;

    public float CurrentStamina { get; private set; }
    public bool HasStamina => CurrentStamina > 0f;

    private bool wasDrainedThisFrame;
    private float targetFillAmount;

    private void Start()
    {
        CurrentStamina = maxStamina;

        targetFillAmount = 1f;

        if (staminaFillImage != null)
        {
            staminaFillImage.fillAmount = 1f;
        }
    }

    private void Update()
    {
        // TEST: Hold Left Shift to drain stamina
        if (Input.GetKey(KeyCode.LeftShift))
        {
            DrainStamina(staminaDrainRate);
        }

        // Smoothly move the bar toward the actual stamina
        if (staminaFillImage != null)
        {
            staminaFillImage.fillAmount = Mathf.Lerp(
                staminaFillImage.fillAmount,
                targetFillAmount,
                smoothSpeed * Time.deltaTime
            );
        }
    }

    private void LateUpdate()
    {
        if (!wasDrainedThisFrame && CurrentStamina < maxStamina)
        {
            CurrentStamina += staminaRegenRate * Time.deltaTime;
            CurrentStamina = Mathf.Min(CurrentStamina, maxStamina);

            UpdateTargetFill();
        }

        wasDrainedThisFrame = false;
    }

    public bool DrainStamina(float amountPerSecond)
    {
        if (CurrentStamina <= 0f)
            return false;

        CurrentStamina -= amountPerSecond * Time.deltaTime;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, maxStamina);

        UpdateTargetFill();

        wasDrainedThisFrame = true;

        return true;
    }

    public bool ConsumeStaminaOnce(float amount)
    {
        if (CurrentStamina < amount)
            return false;

        CurrentStamina -= amount;
        CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, maxStamina);

        UpdateTargetFill();

        wasDrainedThisFrame = true;

        return true;
    }

    private void UpdateTargetFill()
    {
        targetFillAmount = CurrentStamina / maxStamina;
    }
}