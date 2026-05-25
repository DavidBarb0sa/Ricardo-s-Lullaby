using UnityEngine;

public class StartDicaRandom : MonoBehaviour
{
    public GameObject[] dicasPrefabs;
    public Transform dicaSpawnPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnDica();
    }

    void SpawnDica()
    {
        int randomIndex = Random.Range(0, dicasPrefabs.Length);

        GameObject selectedItem = dicasPrefabs[randomIndex];
        Debug.Log("Dica selecionada: " + selectedItem.name);

        // spawn
        Instantiate(
            selectedItem,
            dicaSpawnPoint.position,
            dicaSpawnPoint.rotation
        );
    }
}
