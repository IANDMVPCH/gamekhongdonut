using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoSceneController : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Header("Next Scene")]
    public string nextSceneName;

    private bool sceneChanging = false;

    void Start()
    {
        // Detect when the video finishes
        videoPlayer.loopPointReached += VideoFinished;
    }

    void VideoFinished(VideoPlayer vp)
    {
        LoadNextScene();
    }

    public void SkipVideo()
    {
        LoadNextScene();
    }

    void LoadNextScene()
    {
        if (sceneChanging)
            return;

        sceneChanging = true;

        SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= VideoFinished;
        }
    }
}