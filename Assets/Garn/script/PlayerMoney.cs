using UnityEngine;

public class PlayerMoney : MonoBehaviour
{

    public bool SpendMoney(int amount)
    {
        if (GameData.currentMoney >= amount)
        {
            GameData.currentMoney -= amount;

            Debug.Log("Money left: " + GameData.currentMoney);

            return true;
        }

        Debug.Log("Not enough money!");
        return false;
    }

    public void AddMoney(int amount)
    {
        GameData.currentMoney += amount;

        Debug.Log("Money: " + GameData.currentMoney);
    }

    public int GetMoney()
    {
        return GameData.currentMoney;
    }
}