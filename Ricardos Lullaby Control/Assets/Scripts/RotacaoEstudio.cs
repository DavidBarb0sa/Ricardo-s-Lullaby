using UnityEngine;

// Repara que diz aqui ": MonoBehaviour", por isso ele é um MonoBehaviour!
public class RotacaoEstudio : MonoBehaviour
{
    [Header("Configuração do Giro")]
    public float velocidadeRotacao = 30f;

    void Update()
    {
        // Roda o objeto no eixo Y (vertical) continuamente
        transform.Rotate(Vector3.up, velocidadeRotacao * Time.unscaledDeltaTime);
    }
}