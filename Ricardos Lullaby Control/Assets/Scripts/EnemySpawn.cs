using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject Enemy;
    public float spawnMaxRadius = 40f;
    public float spawnMinRadius = 30f;
    void Start()
    {
        if (Enemy == null)
            Debug.LogWarning("[EnemyAI] Inimigo não encontrado! Certifica-te de que o objeto está atribuído no Inspector.");
    }

    // Update is called once per frame
    void Update()
    {
        if (Enemy.active == false && Random.Range(0f, 1f) < 0.005f)
        {
            Enemy.SetActive(true);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, spawnMaxRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnMinRadius);
    }
}