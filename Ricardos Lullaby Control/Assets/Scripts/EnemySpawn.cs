using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawn : MonoBehaviour
{
    public GameObject Enemy;

    public float spawnMaxRadius = 40f;
    public float spawnMinRadius = 30f;

    public float minTimeDisappear = 15f;

    float disappearTimer = 0f; //Tempo desde a ultima vez que desapareceu

    void Start()
    {
        if (Enemy == null)
        {
            Debug.LogWarning("[EnemyAI] Inimigo não encontrado!");
        }

        
    }

    void Update()
    {
        disappearTimer += Time.deltaTime;
        if ((!Enemy.activeSelf && Random.Range(0f, 1f) < 0.005f) && disappearTimer > minTimeDisappear)
        {
            SpawnEnemy();
            disappearTimer = 0f;
        }
    }

    void SpawnEnemy()
    {
        for (int i = 0; i < 20; i++) // tenta várias vezes
        {
            // Direção aleatória
            Vector2 randomCircle = Random.insideUnitCircle.normalized;

            // Distância aleatória entre min e max
            float randomDistance = Random.Range(spawnMinRadius, spawnMaxRadius);

            // Posição candidata
            Vector3 randomPos = transform.position +
                                new Vector3(randomCircle.x, 0, randomCircle.y) * randomDistance;

            // Procura ponto válido na NavMesh
            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                Enemy.transform.position = hit.position;
                Enemy.SetActive(true);
                return;
            }
        }

        Debug.LogWarning("Não foi encontrada posição válida na NavMesh.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, spawnMaxRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnMinRadius);
    }
}