using UnityEngine;

public class ItemApanhavel : MonoBehaviour
{
    // Aqui vais arrastar o arquivo ItemData que criaste no Passo 1
    public ItemData dadosDoItem;
    
    [Header("Spawn Inimigo")]
    public GameObject enemy;
    public bool spawnaInimigo = false;
}