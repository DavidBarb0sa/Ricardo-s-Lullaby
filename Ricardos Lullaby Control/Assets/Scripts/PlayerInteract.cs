using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 3f; // distancia do pickup
    public Transform holdPoint; // onde o objeto vai ficar (mão)
    public KeyCode interactKey = KeyCode.E; // alterar tecla depois
    public TextMeshProUGUI pickupText;

    private GameObject heldObject;

    void Update()
    {
        // Se NÃO temos nada na mão, podemos tentar apanhar algo (da mão OU do inventário)
        if (heldObject == null)
        {
            TryPickUp();
        }
        else
        {
            // Se já temos algo na mão, ainda queremos poder olhar para itens do inventário e apanhá-los!
            // Por isso corremos uma versão secundária do Raycast só para o inventário se a mão estiver ocupada
            TryPickUpApenasInventario();
        }

        if (heldObject != null && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
    }

    void TryPickUp()
    {
        if (Camera.main == null) return;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        bool isLookingAtPickup = false;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // MÁGICA AQUI: O texto aparece se olhares para um item de MÃO ou de INVENTÁRIO
            if (hit.collider.CompareTag("Pickup") || hit.collider.CompareTag("ItemInventario"))
            {
                isLookingAtPickup = true;

                if (Input.GetKeyDown(interactKey))
                {
                    // CASO 1: É um item para ir para a mão
                    if (hit.collider.CompareTag("Pickup"))
                    {
                        PickUp(hit.collider.gameObject);
                        isLookingAtPickup = false;
                    }
                    // CASO 2: É um item para o Inventário!
                    else if (hit.collider.CompareTag("ItemInventario"))
                    {
                        ApanharParaInventario(hit.collider.gameObject);
                        isLookingAtPickup = false;
                    }
                }
            }
        }

        pickupText.gameObject.SetActive(isLookingAtPickup);
    }

    // Função extra: Permite apanhar papéis/chaves para o inventário mesmo se já estiveres a carregar uma caixa/lanterna na mão
    void TryPickUpApenasInventario()
    {
        if (Camera.main == null) return;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        bool isLookingAtInventoryItem = false;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("ItemInventario"))
            {
                isLookingAtInventoryItem = true;

                if (Input.GetKeyDown(interactKey))
                {
                    ApanharParaInventario(hit.collider.gameObject);
                    isLookingAtInventoryItem = false;
                }
            }
        }

        pickupText.gameObject.SetActive(isLookingAtInventoryItem);
    }

    void PickUp(GameObject obj)
    {
        heldObject = obj;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        obj.transform.SetParent(holdPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
    }

    void ApanharParaInventario(GameObject obj)
    {
        ItemApanhavel itemScript = obj.GetComponent<ItemApanhavel>();

        if (itemScript != null && itemScript.dadosDoItem != null)
        {
              Debug.Log("Item apanhado: " + itemScript.dadosDoItem.nomeItem);
              if (itemScript.spawnaInimigo && itemScript.enemy != null)
            {
                Debug.Log("A tentar spawnar!");
                EnemySpawn spawnScript = itemScript.enemy.GetComponent<EnemySpawn>();
                if (spawnScript != null)
                spawnScript.EnableSpawn();
                else
                Debug.Log("EnemySpawn não encontrado no objeto!");
            }
            if (InventoryManager.Instance != null)
            {
                // MÁGICA: Passamos os dados do item E o próprio objeto físico (obj) da cena!
                InventoryManager.Instance.ApanharEGuardar(itemScript.dadosDoItem, obj);

                // JÁ NÃO DESTROÍMOS O OBJETO! O InventoryManager trata dele.
            }
            else
            {
                Debug.LogError("Não foi encontrado nenhum InventoryManager na cena!");
            }
        }
    }

    void Drop()
    {
        GameObject obj = heldObject;
        heldObject = null;

        obj.transform.SetParent(null);

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
}