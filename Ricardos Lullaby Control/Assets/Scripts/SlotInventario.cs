using UnityEngine;
using UnityEngine.EventSystems; // Necessário para detetar cliques na UI
using UnityEngine.UI;

// Usamos o IPointerClickHandler para o Unity saber quando o rato clica aqui
public class SlotInventario : MonoBehaviour, IPointerClickHandler
{
    private GameObject objetoDesteSlot;
    private ItemData dadosDesteSlot;

    // O InventoryManager vai usar isto para associar o item ao slot quando o apanhas
    public void ConfigurarSlot(GameObject obj, ItemData dados)
    {
        objetoDesteSlot = obj;
        dadosDesteSlot = dados;
    }

    // Esta função corre automaticamente sempre que clicas com o rato neste slot!
    public void OnPointerClick(PointerEventData eventData)
    {
        // Se este slot tiver um item guardado...
        if (objetoDesteSlot != null && InventoryManager.Instance != null)
        {
            // Avisa o Manager para focar este objeto no estúdio grande e atualizar os textos!
            InventoryManager.Instance.FocarObjetoNoEstudio(objetoDesteSlot, dadosDesteSlot);
            Debug.Log("Clicaste no item: " + dadosDesteSlot.nomeItem);
        }
    }
}