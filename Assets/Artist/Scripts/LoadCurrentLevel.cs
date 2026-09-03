using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.GetComponent<HealthPlayer>() != null)
        {
            LoadCurrentLevel();
        }
    }

    public void LoadCurrentLevel()
    {
        // Safety check for empty array
        if (GameData.levelScenes == null || GameData.levelScenes.Length == 0)
        {
            Debug.LogError("GameData.levelScenes array is empty!");
            return;
        }

        // Clamp index to prevent OutOfBounds errors
        int levelIndex = Mathf.Clamp(GameData.currentLevel, 0, GameData.levelScenes.Length - 1);
        string targetScene = GameData.levelScenes[levelIndex];

        Debug.Log($"Loading scene: {targetScene} (Index: {levelIndex})");
        SceneManager.LoadScene(targetScene);
    }

    public void LoadNextLevel()
    {
        GameData.currentLevel++;

        if (GameData.currentLevel >= GameData.levelScenes.Length)
        {
            Debug.Log("All levels completed! Resetting to level 0.");
            GameData.currentLevel = 0;
        }

        LoadCurrentLevel();
    }
}