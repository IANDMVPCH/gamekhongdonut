using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverButton : MonoBehaviour
{
    [Header("Scene To Load")]
    public string sceneName = "Game";

    public void PlayAgain()
    {
        SceneManager.LoadScene(sceneName);
    }
}