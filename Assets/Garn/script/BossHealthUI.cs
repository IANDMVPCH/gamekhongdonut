using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public FinalBoss boss;
    public Image healthFill;

    void Update()
    {
        if (boss == null || healthFill == null)
            return;

        float healthPercent =
            (float)boss.GetCurrentHealth() /
            boss.GetMaxHealth();

        healthFill.fillAmount = healthPercent;
    }
}