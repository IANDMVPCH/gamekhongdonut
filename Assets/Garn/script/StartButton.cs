using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public string sceneName = "MainGame";

    public void StartGame()
    {
        SceneManager.LoadScene(sceneName);
    }
}