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
    public float playerFOVAngle = 90f;
    public float resumeCooldown = 3f;
    public LayerMask obstacleLayers;

    private NavMeshAgent agent;
    private float cooldownTimer;
    private bool isFrozen;

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
    }

    bool PlayerIsLooking()
    {
        Vector3 toEnemy = transform.position - playerCamera.position;
        float angle = Vector3.Angle(playerCamera.forward, toEnemy);
        if (angle > playerFOVAngle * 0.5f) return false;

        if (Physics.Linecast(playerCamera.position,
                             transform.position + Vector3.up, obstacleLayers)) return false;

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

        // FOV do jogador
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Vector3 left  = Quaternion.Euler(0, -playerFOVAngle * 0.5f, 0) * playerCamera.forward;
        Vector3 right = Quaternion.Euler(0,  playerFOVAngle * 0.5f, 0) * playerCamera.forward;
        Gizmos.DrawRay(playerCamera.position, left  * detectionRange);
        Gizmos.DrawRay(playerCamera.position, right * detectionRange);
    }
}