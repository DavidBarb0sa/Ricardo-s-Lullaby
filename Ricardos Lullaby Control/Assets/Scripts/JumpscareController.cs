using UnityEngine;
using System.Collections;

public class JumpscareController : MonoBehaviour
{
    public static JumpscareController Instance;

    [Header("Referências")]
    public Transform inimigoRosto;
    public Camera cameraPrincipal;
    public GameObject gameOverScreen;
    public EfeitoEstatica efeitoEstatica;

    // 1. ADICIONADO: Referência para o componente de Áudio
    public AudioSource audioJumpscare;

    [Header("Movimento")]
    public float duracaoMovimento = 2f;
    public float duracaoTremor = 3f;
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
        if (efeitoEstatica != null)
        {
            efeitoEstatica.imagemEstatica.gameObject.SetActive(false);
            efeitoEstatica.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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

        if (audioJumpscare != null)
        {
            audioJumpscare.Play();
        }

        // --- FASE 2: Tremor na cara do inimigo ---
        Vector3 posicaoFinal = inimigoRosto.position;
        tempo = 0f;
        
        while (tempo < duracaoTremor)
        {
            tempo += Time.unscaledDeltaTime;

            Vector3 tremor = new Vector3(
                Mathf.Sin(tempo * velocidadeTremor) * intensidadeTremor,
                Mathf.Cos(tempo * velocidadeTremor * 1.3f) * intensidadeTremor, // <- CORRIGIDO AQUI
                0f
            );

            cameraPrincipal.transform.position = posicaoFinal + tremor;

            yield return null;
        }

        // Dá 0.1 segundos reais para o motor de som arrancar 
        // antes de congelarmos o motor físico do jogo
        yield return new WaitForSecondsRealtime(0.1f);

        // --- FIM: Mostra o GameOver ---
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
    }
}