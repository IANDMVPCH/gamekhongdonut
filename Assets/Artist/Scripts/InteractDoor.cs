using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractDoor: MonoBehaviour
{
    [Header("Scene")]

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

            GameData.currentLevel++;
            SceneManager.LoadScene(GameData.levelScenes[GameData.currentLevel]);
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
}