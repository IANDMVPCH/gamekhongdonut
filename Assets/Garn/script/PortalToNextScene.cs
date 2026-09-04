using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalToNextScene : MonoBehaviour
{
    [Header("Next Scene")]
    public string nextSceneName = "NextLevel";

    private bool playerEntered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerEntered)
            return;

        if (other.CompareTag("Player"))
        {
            playerEntered = true;

            Debug.Log("Player entered the boss portal!");

            SceneManager.LoadScene(nextSceneName);
        }
    }
}