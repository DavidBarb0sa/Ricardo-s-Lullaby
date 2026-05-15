using UnityEngine;
using System.Collections.Generic;

public class ItemRandomizer : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    public Transform[] spawnPoints;

    void Start()
    {
        SpawnItems();
    }

    void SpawnItems()
    {
        // cria uma lista temporária dos itens
        List<GameObject> availableItems = new List<GameObject>(itemPrefabs);

        foreach (Transform point in spawnPoints)
        {
            // se acabarem os itens, para
            if (availableItems.Count == 0)
                return;

            // escolhe item aleatório
            int randomIndex = Random.Range(0, availableItems.Count);

            GameObject selectedItem = availableItems[randomIndex];

            // spawn
            Instantiate(
                selectedItem,
                point.position,
                point.rotation
            );

            // remove para não repetir
            availableItems.RemoveAt(randomIndex);
            
        }
    }
}