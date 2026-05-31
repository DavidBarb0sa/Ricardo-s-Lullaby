using UnityEngine;

[CreateAssetMenu(fileName = "NovoItem", menuName = "Inventario/Item")]
public class ItemData : ScriptableObject
{
    public string nomeItem;
    [TextArea] public string descricao;
    // Removeu-se a linha do Prefab porque agora usamos o da cena!
}