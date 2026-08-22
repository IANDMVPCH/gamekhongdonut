using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int itemCount = 0;

    public void AddItem(int amount)
    {
        itemCount += amount;
        Debug.Log("Items: " + itemCount);
    }
}