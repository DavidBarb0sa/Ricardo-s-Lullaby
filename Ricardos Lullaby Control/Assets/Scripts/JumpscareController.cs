using UnityEngine;
using System.Collections;

public class JumpscareController : MonoBehaviour
{
    public static JumpscareController Instance;

    [Header("Referências")]
    public Transform inimigoRosto;       // ponto na cara do inimigo (cria um Transform filho no inimigo)
    public Camera cameraPrincipal;
    public GameObject gameOverScreen;
    public EfeitoEstatica efeitoEstatica; // para desligar a estática durante o jumpscare

    [Header("Movimento")]
    public float duracaoMovimento = 2f;  // tempo a ir para a cara do inimigo
    public float duracaoTremor = 3f;     // tempo a tremer na cara do inimigo
    public float intensidadeTremor = 0.05f;
    public float velocidadeTremor = 30f;

    private bool emJumpscare = false;

    void Awake()
    {
        Instance = this;
    }

    public void IniciarJumpscare()
    {
        if (emJumpscare) return;
        emJumpscare = true;
        StartCoroutine(RotinajumpScare());
    }

    private IEnumerator RotinajumpScare()
    {
        // Desliga a estática imediatamente
        if (efeitoEstatica != null)
        {
            efeitoEstatica.imagemEstatica.gameObject.SetActive(false);
            efeitoEstatica.enabled = false;
        }

        // Desbloqueia o rato e desativa o script de movimento do jogador
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Desativa scripts do jogador para não interferir com a câmara
        if (cameraPrincipal != null)
        {
            MonoBehaviour[] scripts = cameraPrincipal.GetComponentsInParent<MonoBehaviour>();
            foreach (MonoBehaviour s in scripts)
            {
                if (s != this) s.enabled = false;
            }
        }

        // --- FASE 1: Move a câmara suavemente para a cara do inimigo ---
        Vector3 posicaoInicial = cameraPrincipal.transform.position;
        Quaternion rotacaoInicial = cameraPrincipal.transform.rotation;

        float tempo = 0f;
        while (tempo < duracaoMovimento)
        {
            tempo += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, tempo / duracaoMovimento);

            cameraPrincipal.transform.position = Vector3.Lerp(posicaoInicial, inimigoRosto.position, t);
            cameraPrincipal.transform.rotation = Quaternion.Slerp(rotacaoInicial, inimigoRosto.rotation, t);

            yield return null;
        }

        // --- FASE 2: Tremor na cara do inimigo ---
        Vector3 posicaoFinal = inimigoRosto.position;
        tempo = 0f;
        while (tempo < duracaoTremor)
        {
            tempo += Time.unscaledDeltaTime;

            Vector3 tremor = new Vector3(
                Mathf.Sin(tempo * velocidadeTremor) * intensidadeTremor,
                Mathf.Cos(tempo * velocidadeTremor * 1.3f) * intensidadeTremor,
                0f
            );

            cameraPrincipal.transform.position = posicaoFinal + tremor;

            yield return null;
        }

        // --- FIM: Mostra o GameOver ---
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
    }
}