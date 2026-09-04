using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorInteract : MonoBehaviour
{
    [Header("Scene")]
    public string nextSceneName = "Level2";

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;

    private bool playerNearby = false;

    private void OnEnable()
    {
        playerNearby = false;
    }

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(interactKey))
        {
            Debug.Log("DOOR: E pressed!");

            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            Debug.Log("DOOR: Player is near the door!");
            Debug.Log("DOOR: Press E to enter.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            Debug.Log("DOOR: Player left the door.");
        }
    }
}