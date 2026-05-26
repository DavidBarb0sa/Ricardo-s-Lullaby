using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasJogo : MonoBehaviour
{
    public void VoltaMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}