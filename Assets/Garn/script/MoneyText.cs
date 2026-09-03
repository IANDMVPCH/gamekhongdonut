using UnityEngine;
using TMPro;

public class MoneyText : MonoBehaviour
{
    public PlayerMoney playerMoney;
    public TMP_Text moneyText;

    void Start()
    {
        UpdateMoneyText();
    }

    void Update()
    {
        UpdateMoneyText();
    }

    void UpdateMoneyText()
    {
        if (playerMoney == null || moneyText == null)
            return;

        moneyText.text = "$" + playerMoney.GetMoney();
    }
}