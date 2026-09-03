using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject nextLevelPortal;

    private int enemyCount;

    void Start()
    {
        // Find every EnemyHealth in the scene
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);

        enemyCount = enemies.Length;

        // Hide portal at the beginning
        if (nextLevelPortal != null)
        {
            nextLevelPortal.SetActive(false);
        }

        Debug.Log("Enemies remaining: " + enemyCount);
    }

    public void EnemyKilled()
    {
        enemyCount--;

        Debug.Log("Enemies remaining: " + enemyCount);

        if (enemyCount <= 0)
        {
            OpenPortal();
        }
    }

    void OpenPortal()
    {
        if (nextLevelPortal != null)
        {
            nextLevelPortal.SetActive(true);
        }

        Debug.Log("ALL ENEMIES DEAD! PORTAL OPENED!");
    }
}