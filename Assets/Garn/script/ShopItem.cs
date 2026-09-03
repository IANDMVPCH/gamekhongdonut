using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [Header("Item Information")]
    public string itemName = "Health Potion";
    public int price = 25;

    [Header("Item To Give")]
    public GameObject itemPrefab;

    [Header("Purchase")]
    public bool purchased = false;

    public void BuyItem()
    {
        // Already bought
        if (purchased)
        {
            Debug.Log("You already bought " + itemName + "!");
            return;
        }

        PlayerMoney playerMoney = FindFirstObjectByType<PlayerMoney>();

        if (playerMoney == null)
        {
            Debug.LogError("PlayerMoney was not found!");
            return;
        }

        // Check if player has enough money
        if (playerMoney.SpendMoney(price))
        {
            // Give the item
            GiveItem();

            // Mark as purchased
            purchased = true;

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