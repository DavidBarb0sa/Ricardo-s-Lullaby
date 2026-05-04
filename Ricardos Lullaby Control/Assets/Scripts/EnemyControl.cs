using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyControl : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public Transform playerCamera;

    [Header("Deteção")]
    public float detectionRange = 15f;

    public float resumeCooldown = 3f;
    public LayerMask obstacleLayers;

    private NavMeshAgent agent;
    private float cooldownTimer;
    private bool isFrozen;

    [Header("Game Over")]
    public GameObject gameOverScreen;
    public float killTime = 5f;

    private float touchTimer = 0f;
    private bool isTouchingPlayer = false;
    

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Tenta encontrar automaticamente o player pela tag
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;

                // Tenta encontrar a camera automaticamente dentro do player
                if (playerCamera == null)
                {
                    Camera cam = playerObj.GetComponentInChildren<Camera>();
                    if (cam != null)
                        playerCamera = cam.transform;
                }
            }
            else
            {
                Debug.LogWarning("[EnemyAI] Player não encontrado! Certifica-te que tem a tag 'Player'.");
            }
        }

        if (playerCamera == null)
            Debug.LogWarning("[EnemyAI] Player Camera não encontrada! Arrasta a câmara no Inspector.");
    }

    void Update()
    {
        if (player == null || playerCamera == null) return;


        if (PlayerIsLooking())
        {
            // Para imediatamente
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            isFrozen = true;
            cooldownTimer = resumeCooldown;
        }
        else if (isFrozen)
        {
            // Cooldown antes de voltar a perseguir
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
                isFrozen = false;
        }
        else
        {
            // Persegue o jogador
            if (Vector3.Distance(transform.position, player.position) <= detectionRange)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
        }
        HandleGameOver();
    }

 bool PlayerIsLooking()
{
    Vector3 viewportPos = playerCamera.GetComponent<Camera>().WorldToViewportPoint(transform.position);

    // verifica se esta a frente da camera
    if (viewportPos.z < 0)
        return false;

    // margem de erro
    float margin = 0f; // aumentar margem
    if (viewportPos.x < 0f - margin || viewportPos.x > 1f + margin ||
        viewportPos.y < 0f - margin || viewportPos.y > 1f + margin)
        return false;

    // Verificar se há obstáculos
    if (Physics.Linecast(playerCamera.position,
                         transform.position + Vector3.up,
                         obstacleLayers))
        return false;

    return true;
}

    // Gizmos para debug na Scene View
    void OnDrawGizmosSelected()
    {
        // Alcance de deteção
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (!Application.isPlaying || playerCamera == null) return;

        // Linha entre inimigo e câmara
        Gizmos.color = isFrozen ? Color.cyan : Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up, playerCamera.position);

    }

    void OnCollisionStay(Collision collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        isTouchingPlayer = true;
    }
}

void OnCollisionExit(Collision collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        isTouchingPlayer = false;
        touchTimer = 0f; // reset se sair
    }
}

void HandleGameOver()
{
    if (isTouchingPlayer)
    {
        touchTimer += Time.deltaTime;

        if (touchTimer >= killTime)
        {
            GameOver();
        }
    }
}

void GameOver()
{
    gameOverScreen.SetActive(true);
    Time.timeScale = 0f;
}
}

