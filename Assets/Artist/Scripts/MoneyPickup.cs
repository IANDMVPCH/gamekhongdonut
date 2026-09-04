using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    [Header("Money Settings")]
    [Tooltip("Amount of money given on pickup.")]
    public int moneyAmount = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMoney playerMoney = other.GetComponent<PlayerMoney>();

        if (playerMoney != null)
        {
            playerMoney.AddMoney(moneyAmount);
            Destroy(gameObject);
        }
    }
}