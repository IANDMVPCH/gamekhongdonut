using UnityEngine;

public class ActivateAfterEnemiesDead : MonoBehaviour
{
    public GameObject objectToActivate;
    public GameObject[] enemies;

    void Update()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
                return;
        }

        objectToActivate.SetActive(true);
        enabled = false;
    }
}