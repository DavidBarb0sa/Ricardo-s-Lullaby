using UnityEngine;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Coisas do Menu")]
    public GameObject canvasMenu;
    public Camera cameraMenu;
    public GameObject luzDoMenu;

    [Header("Coisas do Jogo")]
    public GameObject jogadorPrincipal; // O teu Player
    public GameObject Ricardo;          // O Ricardo, o teu melhor amigo(confia)
    public GameObject canvasJogo;       // A UI da bateria e interações

    void Start()
    {
        // Quando o jogo arranca, garante que o menu está ativo
        canvasMenu.SetActive(true);
        cameraMenu.gameObject.SetActive(true);
        luzDoMenu.SetActive(true);
        
        // E garante que o jogador, o Ricardo e a UI do jogo estão desligados
        jogadorPrincipal.SetActive(false);
        Ricardo.SetActive(false);
        canvasJogo.SetActive(false);

        // Mostra o rato para o jogador poder clicar nos botões
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ComecarJogo()
    {
        // 1. Desliga o menu
        canvasMenu.SetActive(false);
        cameraMenu.gameObject.SetActive(false);
        luzDoMenu.SetActive(false);

        // 2. Liga o Jogador, o Ricardo e a UI do jogo
        jogadorPrincipal.SetActive(true);
        Ricardo.SetActive(true);
        canvasJogo.SetActive(true);

        // 3. Esconde e tranca o rato para o modo de terror (1ª pessoa)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void AbrirOpcoes()
    {
        Debug.Log("Abrir o painel de opções!");
    }

    public void SairDoJogo()
    {
        Debug.Log("A sair do jogo...");
        Application.Quit();
    }
}