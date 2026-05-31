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

            objetoDaCena.SetActive(true);
            SlotInventario scriptSlot = slotLivre.GetComponent<SlotInventario>();
            if (scriptSlot != null) scriptSlot.ConfigurarSlot(objetoDaCena, dados);

            FocarObjetoNoEstudio(objetoDaCena, dados);

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
                if (imagemDoSlot != null) imagemDoSlot.texture = fotoDoItem;
            }

            objetoDaCena.SetActive(false);
            if (objetoFocado != null && inventarioAberto) objetoFocado.SetActive(true);
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

        // 1. Guardamos a rotação global atual antes de mudar de pai
        Quaternion rotacaoOriginal = obj.transform.rotation;

        // 2. Mudamos o pai
        obj.transform.SetParent(pontoSpawnEstudio);

        // 3. Resetamos a posição local para ele ir para o centro, mas mantemos a rotação
        obj.transform.localPosition = Vector3.zero;

        // 4. Aplicamos a rotação original que guardamos
        obj.transform.rotation = rotacaoOriginal;

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
        {
            if (objetoFocado != textoNomeItem.gameObject && objetoFocado != textoDescricaoItem.gameObject)
            {
                objetoFocado.SetActive(inventarioAberto);
            }
        }

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