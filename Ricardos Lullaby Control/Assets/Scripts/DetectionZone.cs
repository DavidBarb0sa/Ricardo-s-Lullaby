using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoalZone : MonoBehaviour
{
    public int requiredObjects = 6; // número de objetos para ganhar
    public GameObject winScreen;
    private HashSet<GameObject> objectsInZone = new HashSet<GameObject>();
    public EnemyControl enemy;
    public float speedIncrease = 0.5f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            if (!objectsInZone.Contains(other.gameObject))
            {
                objectsInZone.Add(other.gameObject);

                   if (enemy != null)
                enemy.GetComponent<NavMeshAgent>().speed += speedIncrease;

                Debug.Log("Objetos na zona: " + objectsInZone.Count);

                if (objectsInZone.Count >= requiredObjects)
                {
                    WinGame();
                }
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
      if (other.CompareTag("Pickup"))
      {
        objectsInZone.Remove(other.gameObject);
        Debug.Log("Objetos na zona: " + objectsInZone.Count);
      }
    }

    void WinGame()
    {
        winScreen.SetActive(true);
        Time.timeScale = 0f; 
    }
}