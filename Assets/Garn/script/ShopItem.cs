using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [Header("Item Information")]
    public string itemName = "Health Potion";
    public int price = 25;

    [Header("Item To Give")]
    public GameObject itemPrefab;

    public void BuyItem()
    {
        PlayerMoney playerMoney = FindFirstObjectByType<PlayerMoney>();

        if (playerMoney == null)
        {
            Debug.LogError("PlayerMoney was not found!");
            return;
        }

        if (playerMoney.SpendMoney(price))
        {
            GiveItem();

            Debug.Log("Bought: " + itemName);
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    void GiveItem()
    {
        if (itemPrefab != null)
        {
            Instantiate(
                itemPrefab,
                playerMoneyPosition(),
                Quaternion.identity
            );
        }
    }

    Vector3 playerMoneyPosition()
    {
        PlayerMoney player = FindFirstObjectByType<PlayerMoney>();

        if (player != null)
        {
            return player.transform.position;
        }

        return Vector3.zero;
    }
}