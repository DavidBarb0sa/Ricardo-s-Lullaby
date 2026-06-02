using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EfeitoEstatica : MonoBehaviour
{
    [Header("Referências")]
    public Transform inimigo;
    public RawImage imagemEstatica; // RawImage no Canvas com uma textura de ruído/estática

    [Header("Distâncias")]
    public float distanciaMaxima = 15f; // a partir desta distância começa o efeito
    public float distanciaMinima = 2f;  // a esta distância o efeito está no máximo

    [Header("Intensidade")]
    public float alphaMinimo = 0f;
    public float alphaMaximo = 0.85f;

    [Header("Animação da Estática")]
    public float velocidadeAnimacao = 0.05f; // velocidade a que os tiles da textura mudam

    private Transform player;
    private float timerAnimacao;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (imagemEstatica != null)
            imagemEstatica.color = new Color(1, 1, 1, 0);
    }

    void Update()
    {
        if (player == null || inimigo == null || imagemEstatica == null) return;

        // Só aplica efeito se o inimigo estiver ativo
        if (!inimigo.gameObject.activeInHierarchy)
        {
            imagemEstatica.color = new Color(1, 1, 1, 0);
            return;
        }

        float distancia = Vector3.Distance(player.position, inimigo.position);

        // Calcula o alpha com base na distância (quanto mais perto, mais forte)
        float t = 1f - Mathf.Clamp01((distancia - distanciaMinima) / (distanciaMaxima - distanciaMinima));
        float alpha = Mathf.Lerp(alphaMinimo, alphaMaximo, t);

        imagemEstatica.color = new Color(1, 1, 1, alpha);

        // Anima a textura para dar sensação de estática a mover
        if (alpha > 0f)
        {
            timerAnimacao += Time.deltaTime;
            if (timerAnimacao >= velocidadeAnimacao)
            {
                timerAnimacao = 0f;
                imagemEstatica.uvRect = new Rect(
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    imagemEstatica.uvRect.width,
                    imagemEstatica.uvRect.height
                );
            }
        }
    }
}