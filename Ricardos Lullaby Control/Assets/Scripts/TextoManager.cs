using UnityEngine;
using TMPro;
using System.Collections;

public class TextoInicial : MonoBehaviour
{
    public TextMeshProUGUI textoDialogo;
    public float velocidadeEscrita = 0.05f;
    public float tempoParaDesaparecer = 4f;

    private string mensagem = "Devo ter deixado a carteira na sala da tuna.";

    void Start()
    {
        if (textoDialogo != null)
            StartCoroutine(EscreverTexto());
    }

    private IEnumerator EscreverTexto()
    {
        textoDialogo.text = "";
        foreach (char letra in mensagem)
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadeEscrita);
        }
        yield return new WaitForSeconds(tempoParaDesaparecer);
        textoDialogo.text = "";
    }
}