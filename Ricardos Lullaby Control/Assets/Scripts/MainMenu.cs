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
    public GameObject menuInterface;
    public GameObject menuOpcoes;

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        videoPlayer.Stop();
        videoPlayer.frame = 0;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture.Release();
    }

    public void Options()
    {
        menuInterface.SetActive(false);
        menuOpcoes.SetActive(true);
    }
    public void Back()
    {
        menuOpcoes.SetActive(false);
        menuInterface.SetActive(true);
    }
    public void QuitGame()
    {
        //Application.Quit();
        Debug.Log("Quitted game!");
    }
}
