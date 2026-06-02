using Unity.VisualScripting;
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
    public float maxFreezeDistance = 13f;
    public LayerMask obstacleLayers;
    public float minTimeSpawn = 20f;

    private NavMeshAgent agent;
    private float cooldownTimer;
    private bool isFrozen;
    private bool isDistracted = false;

    [Header("Game Over")]
    public GameObject gameOverScreen;
    public float killTime = 5f;

    private float touchTimer = 0f;
    private bool isTouchingPlayer = false;
    private float spawnTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;

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
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (player == null || playerCamera == null) return;

        if ((PlayerIsLooking() && distanceToPlayer <= maxFreezeDistance))
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            isFrozen = true;
            cooldownTimer = resumeCooldown;
        }
        else if (isFrozen)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
                isFrozen = false;
        }
        else
        {
            spawnTimer += Time.deltaTime;

            if (Vector3.Distance(transform.position, player.position) <= detectionRange)
            {
                agent.isStopped = false;

                if (!isDistracted)
                    agent.SetDestination(player.position);

                if (Random.Range(0f, 1f) < 0.0005f && spawnTimer >= minTimeSpawn)
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
        HandleGameOver();
    }

    bool PlayerIsLooking()
    {
        Vector3 viewportPos = playerCamera.GetComponent<Camera>().WorldToViewportPoint(transform.position);

        if (viewportPos.z < 0)
            return false;

        float margin = 0f;
        if (viewportPos.x < 0f - margin || viewportPos.x > 1f + margin ||
            viewportPos.y < 0f - margin || viewportPos.y > 1f + margin)
            return false;

        if (Physics.Linecast(playerCamera.position,
                             transform.position + Vector3.up,
                             obstacleLayers))
            return false;

        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (!Application.isPlaying || playerCamera == null) return;

        Gizmos.color = isFrozen ? Color.cyan : Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up, playerCamera.position);
    }

    void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isTouchingPlayer = true;
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
            touchTimer = 0f;
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
        // Chama o jumpscare em vez do GameOver direto
        if (JumpscareController.Instance != null)
        {
            JumpscareController.Instance.IniciarJumpscare();
        }
        else
        {
            // Fallback se não houver JumpscareController
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            gameOverScreen.SetActive(true);
            Time.timeScale = 0f;
        }

        // Evita chamar GameOver múltiplas vezes
        isTouchingPlayer = false;
        touchTimer = 0f;
    }

    public void GoToPosition(Vector3 position)
    {
        isFrozen = false;
        cooldownTimer = 0f;
        isDistracted = true;
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    public void ResumeChasing()
    {
        isDistracted = false;
        cooldownTimer = 0f;
    }
}