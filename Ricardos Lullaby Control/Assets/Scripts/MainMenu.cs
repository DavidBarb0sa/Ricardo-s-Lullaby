using System.Collections;
using JetBrains.Annotations;
using UnityEditor.Recorder.Encoder;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public GameObject playButton;
    public GameObject quitButton;

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
