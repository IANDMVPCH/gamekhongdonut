using System.Collections.Generic;
using UnityEngine;

public class HealthEnemy : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;

    private int currentHealth;

    [System.Serializable]
    public class LootItem
    {
        public GameObject item;

        [Min(0)]
        public float probability = 10f;
    }

    [Header("Loot Table")]
    public List<LootItem> lootTable = new List<LootItem>();

    [Header("Drop Settings")]
    [Range(0f, 1f)]
    public float dropChance = 1f;

    public Transform dropPoint;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("Enemy took " + damage + " damage!");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        RollForLoot();

        Destroy(gameObject);
    }

    void RollForLoot()
    {
        // Chance that the enemy drops anything at all
        if (Random.value > dropChance)
            return;

        LootItem selectedItem = GetRandomLoot();

        if (selectedItem == null || selectedItem.item == null)
            return;

        Vector3 spawnPosition = dropPoint != null
            ? dropPoint.position
            : transform.position;

        Instantiate(
            selectedItem.item,
            spawnPosition,
            Quaternion.identity
        );
    }

    LootItem GetRandomLoot()
    {
        if (lootTable.Count == 0)
            return null;

        float totalWeight = 0f;

        // Calculate total probability
        foreach (LootItem loot in lootTable)
        {
            if (loot.item != null)
            {
                totalWeight += loot.probability;
            }
        }

        if (totalWeight <= 0f)
            return null;

        // Pick a random value within the total weight
        float randomValue = Random.Range(0f, totalWeight);

        foreach (LootItem loot in lootTable)
        {
            if (loot.item == null)
                continue;

            randomValue -= loot.probability;

            if (randomValue <= 0f)
            {
                return loot;
            }
        }

        return null;
    }
}