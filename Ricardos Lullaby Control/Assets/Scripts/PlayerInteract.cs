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
        if (heldObject == null)
        {
            TryPickUp();
        }

        if (heldObject != null && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
    }

    void TryPickUp()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        


         if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                pickupText.gameObject.SetActive(true); // mostra o texto

                if (Input.GetKeyDown(interactKey))
                {
                    PickUp(hit.collider.gameObject);
                    pickupText.gameObject.SetActive(false); // esconde ao pegar
                }
            }
            else
            {
                pickupText.gameObject.SetActive(false); // esconde se não é pickup
            }
        }
        else
        {
            pickupText.gameObject.SetActive(false); // esconde se nao acerta em nada
        }
    }

void PickUp(GameObject obj)
{
    heldObject = obj;
    Collider playerCol = GetComponent<Collider>();
    Collider objCol = obj.GetComponent<Collider>();

     // Remove a colisao do objeto quando pegas para nao ser projetado para tras
     // para quem estiver a ler isto, este if de colisao assim como na funçao drop ja não é "necessaio"
     // porque eu desativei as fisicas de colisao entres Layer 
     // mas so descobri isso depois, vou deixar ficar para ja just in case ne
   /* if (playerCol != null && objCol != null)
    {
        Physics.IgnoreCollision(objCol, playerCol, true);
    }*/
     
     // Remove a fisica do objeto
    Rigidbody rb = obj.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.isKinematic = true;
        rb.useGravity = false;
    }
 
      // coloca na mao 
    obj.transform.SetParent(holdPoint);
    obj.transform.localPosition = Vector3.zero;
    obj.transform.localRotation = Quaternion.identity;
}

void Drop()
{
    GameObject obj = heldObject;
    heldObject = null; 

  /*  // volta a ativar a colisao
    Collider playerCol = GetComponent<Collider>();
    Collider objCol = obj.GetComponent<Collider>();
    if (playerCol != null && objCol != null)
    {
        Physics.IgnoreCollision(objCol, playerCol, false); 
    }*/

    obj.transform.SetParent(null);

    Rigidbody rb = obj.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }
}
}   