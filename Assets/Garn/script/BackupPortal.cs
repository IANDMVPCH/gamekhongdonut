using UnityEngine;
using UnityEngine.SceneManagement;

public class BackupPortal : MonoBehaviour
{
    [Header("Main Game")]
    public string mainGameSceneName = "MainGame";

    [Header("Player Health")]
    public int returnHealth = 50;

    private bool isReturning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isReturning)
            return;

        if (other.CompareTag("Player"))
        {
            isReturning = true;

            PlayerHealth health = other.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.SetHealth(returnHealth);
            }

            SceneManager.LoadScene(mainGameSceneName);
        }
    }
}