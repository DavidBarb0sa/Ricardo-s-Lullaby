using UnityEngine;
using UnityEngine.UI;

public class MenuOpcoes : MonoBehaviour
{
    public Slider sliderVolume;

    void Start()
    {
        // Carrega o volume guardado, ou 1 (máximo) se for a primeira vez
        float volumeGuardado = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = volumeGuardado;
        sliderVolume.value = volumeGuardado;

        // Quando o slider muda, chama AtualizarVolume
        sliderVolume.onValueChanged.AddListener(AtualizarVolume);
    }

    public void AtualizarVolume(float valor)
    {
        AudioListener.volume = valor;
        PlayerPrefs.SetFloat("Volume", valor); // guarda para a próxima sessão
    }
}