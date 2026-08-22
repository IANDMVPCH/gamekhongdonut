using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    public int money = 100;

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;

            Debug.Log("Money left: " + money);

            return true;
        }

        Debug.Log("Not enough money!");
        return false;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }
}