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
        bool videoAcabou = false;

        // Dizemos ao Unity para nos avisar quando o vídeo chegar ao fim
        videoPlayer.loopPointReached += (vp) => { videoAcabou = true; };
        
        videoPlayer.Play();

        // Espera até que o Unity confirme que o vídeo realmente terminou
        while (!videoAcabou)
        {
            yield return null;
        }

        LoadingScreen.SetActive(true);
        ChangeScene();
    }

    private void ChangeScene()
    {
        Time.timeScale = 1f;
        Debug.Log("1");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("2");
        videoPlayer.Stop();
        Debug.Log("3");
        videoPlayer.frame = 0;
        Debug.Log("4");
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        Debug.Log("5");
        videoPlayer.targetTexture.Release();
        Debug.Log("6");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitted game!");
    }
}
