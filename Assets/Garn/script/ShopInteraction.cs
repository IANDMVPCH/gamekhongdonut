using UnityEngine;

public class ShopInteraction : MonoBehaviour
{
    [Header("Shop UI")]
    public GameObject shopCanvas;

    private bool playerNearby = false;

    void Start()
    {
        shopCanvas.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            shopCanvas.SetActive(true);
        }

        if (shopCanvas.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            shopCanvas.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            // Close shop when player walks away
            shopCanvas.SetActive(false);
        }
    }
}