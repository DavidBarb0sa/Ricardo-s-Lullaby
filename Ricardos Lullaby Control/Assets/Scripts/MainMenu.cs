using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public GameObject playButton;
    public GameObject quitButton;
    public GameObject LoadingScreen;

    public void PlayGame()
    {
        playButton.SetActive(false);
        quitButton.SetActive(false);
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        videoPlayer.Play();
        yield return new WaitForSeconds((float) videoPlayer.clip.length);
        LoadingScreen.SetActive(true);
        ChangeScene();
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        videoPlayer.Stop();
        videoPlayer.frame = 0;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture.Release();
    }

    public void QuitGame()
    {
        //Application.Quit();
        Debug.Log("Quitted game!");
    }
}
