using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Configuração da UI")]
    public GameObject janelaInventario;
    public Transform painelGrelhaEsquerda;
    public RawImage painelItemGrande;

    [Header("Textos do Item")]
    public TextMeshProUGUI textoNomeItem;
    public TextMeshProUGUI textoDescricaoItem;

    [Header("Estúdio 3D")]
    public Transform pontoSpawnEstudio;
    public Camera camEstudio;

    [Header("Controlo de Rotação")]
    public float velocidadeRotacao = 200f;

    private Dictionary<Transform, GameObject> slotsOcupados = new Dictionary<Transform, GameObject>();
    private GameObject objetoFocado;
    public bool inventarioAberto = false;
    private CanvasGroup canvasGrupoInventario;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (janelaInventario != null)
        {
            canvasGrupoInventario = janelaInventario.GetComponentInParent<CanvasGroup>();
            if (canvasGrupoInventario == null)
                canvasGrupoInventario = janelaInventario.GetComponent<CanvasGroup>();
            if (canvasGrupoInventario == null)
                canvasGrupoInventario = janelaInventario.AddComponent<CanvasGroup>();

            janelaInventario.SetActive(true);
            MostrarInventario(false);
        }

        inventarioAberto = false;
        ConfigurarGrelhaInicial();
        LimparTextos();
    }

    void Update()
    {
        // Apenas rotação manual com o rato
        if (inventarioAberto && objetoFocado != null && Input.GetMouseButton(0))
        {
            float rotX = Input.GetAxis("Mouse X") * velocidadeRotacao * Time.unscaledDeltaTime;
            float rotY = Input.GetAxis("Mouse Y") * velocidadeRotacao * Time.unscaledDeltaTime;

            objetoFocado.transform.Rotate(Vector3.up, -rotX, Space.World);
            objetoFocado.transform.Rotate(Vector3.right, rotY, Space.World);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    private void MostrarInventario(bool mostrar)
    {
        if (canvasGrupoInventario == null) return;
        canvasGrupoInventario.alpha = mostrar ? 1f : 0f;
        canvasGrupoInventario.interactable = mostrar;
        canvasGrupoInventario.blocksRaycasts = mostrar;
    }

    private void ConfigurarGrelhaInicial()
    {
        if (painelGrelhaEsquerda == null) return;
        foreach (Transform slot in painelGrelhaEsquerda)
        {
            slotsOcupados[slot] = null;
            RawImage img = slot.GetComponent<RawImage>();
            if (img != null) img.texture = null;
        }
    }

    public void ApanharEGuardar(ItemData dados, GameObject objetoDaCena)
    {
        Transform slotLivre = EncontrarSlotLivre();
        if (slotLivre != null)
        {
            slotsOcupados[slotLivre] = objetoDaCena;

            Rigidbody rb = objetoDaCena.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            SlotInventario scriptSlot = slotLivre.GetComponent<SlotInventario>();
            if (scriptSlot != null)
                scriptSlot.ConfigurarSlot(objetoDaCena, dados);
            else
                Debug.LogError("O slot não tem o script SlotInventario!");

            objetoDaCena.SetActive(true);

            // Posiciona no estúdio SEM qualquer rotação
            objetoDaCena.transform.SetParent(pontoSpawnEstudio);
            objetoDaCena.transform.localPosition = Vector3.zero;
            objetoDaCena.transform.localRotation = Quaternion.identity; // rotação a zero para a foto

            MudarLayerRecursivamente(objetoDaCena, LayerMask.NameToLayer("ItemEstudio"));

            if (dados != null)
            {
                if (textoNomeItem != null) textoNomeItem.text = dados.nomeItem;
                if (textoDescricaoItem != null) textoDescricaoItem.text = dados.descricao;
            }

            // Tira a foto com o item sem rotação
            if (camEstudio != null && painelItemGrande != null && painelItemGrande.texture != null)
            {
                RenderTexture rtBase = (RenderTexture)painelItemGrande.texture;
                RenderTexture.active = rtBase;
                camEstudio.targetTexture = rtBase;
                camEstudio.Render();

                Texture2D fotoDoItem = new Texture2D(rtBase.width, rtBase.height, TextureFormat.RGB24, false);
                fotoDoItem.ReadPixels(new Rect(0, 0, rtBase.width, rtBase.height), 0, 0);
                fotoDoItem.Apply();

                RenderTexture.active = null;

                RawImage imagemDoSlot = slotLivre.GetComponent<RawImage>();
                if (imagemDoSlot != null)
                    imagemDoSlot.texture = fotoDoItem;
                else
                    Debug.LogWarning("O slot não tem RawImage para mostrar a foto!");
            }
            else
            {
                Debug.LogWarning("camEstudio ou painelItemGrande não estão configurados — foto não tirada.");
            }

            // Esconde até o inventário abrir
            objetoDaCena.SetActive(false);
            objetoFocado = objetoDaCena;

            Debug.Log("Objeto guardado e fotografado: " + slotLivre.name);
        }
        else
        {
            Debug.LogWarning("Nenhum slot livre encontrado!");
        }
    }

    private Transform EncontrarSlotLivre()
    {
        if (painelGrelhaEsquerda == null) return null;
        foreach (Transform slot in painelGrelhaEsquerda)
        {
            if (slotsOcupados.ContainsKey(slot) && slotsOcupados[slot] == null) return slot;
        }
        return null;
    }

    public void FocarObjetoNoEstudio(GameObject obj, ItemData dados)
    {
        if (objetoFocado != null) objetoFocado.SetActive(false);
        objetoFocado = obj;

        obj.transform.SetParent(pontoSpawnEstudio);
        obj.transform.localPosition = Vector3.zero;
        // Mantém a rotação atual do objeto — não repõe para zero ao clicar no slot
        // assim a rotação manual do jogador é preservada

        MudarLayerRecursivamente(obj, LayerMask.NameToLayer("ItemEstudio"));

        if (dados != null)
        {
            if (textoNomeItem != null) textoNomeItem.text = dados.nomeItem;
            if (textoDescricaoItem != null) textoDescricaoItem.text = dados.descricao;
        }

        if (inventarioAberto) obj.SetActive(true);
    }

    private void LimparTextos()
    {
        if (textoNomeItem != null) textoNomeItem.text = "";
        if (textoDescricaoItem != null) textoDescricaoItem.text = "";
    }

    private void MudarLayerRecursivamente(GameObject obj, int novaLayer)
    {
        obj.layer = novaLayer;
        foreach (Transform child in obj.transform)
        {
            MudarLayerRecursivamente(child.gameObject, novaLayer);
        }
    }

    public void ToggleInventory()
    {
        inventarioAberto = !inventarioAberto;
        MostrarInventario(inventarioAberto);

        if (objetoFocado != null)
            objetoFocado.SetActive(inventarioAberto);

        if (!inventarioAberto)
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            LimparTextos();
        }
        else
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}