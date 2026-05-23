using UnityEngine;
using TMPro;

public class Charger : MonoBehaviour
{
    [Header("Carregamento")]
    public float chargeAmount = 20f;
    public float chargeTime = 5f;
    public float interactDistance = 2f;

    [Header("UI")]
    public TextMeshProUGUI promptText; // texto "Prima E para carregar"

    private Transform player;
    private bool isCharging = false;
    private float chargeTimer = 0f;

    private float cooldown = 30f;
    private float cooldownTimer = 0f;
    private bool onCooldown = false;

    // NOVIDADE: Variável partilhada por todas as tomadas para saber quem controla a UI
    private static Charger activeCharger = null;

    void Awake ()
    {
        player = GameObject.FindWithTag("Player").transform;
        
        // Apenas a primeira tomada a carregar precisa de esconder o texto no início
        if (promptText != null && activeCharger == null)
            promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (onCooldown)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                onCooldown = false;
                
                // Correção: Procura o Renderer no próprio objeto ou nos filhos (modelos 3D)
                Renderer rend = GetComponentInChildren<Renderer>();
                if (rend != null) rend.material.color = Color.white; 
            }
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        bool isNear = distance <= interactDistance;

        // 1. Se estou perto, e a UI está livre (ou já é minha), eu assumo o controlo!
        if (isNear && (activeCharger == null || activeCharger == this))
        {
            activeCharger = this;

            if (promptText != null)
                promptText.gameObject.SetActive(!isCharging);

            if (Input.GetKeyDown(KeyCode.E) && !isCharging)
            {
                StartCharging();
            }

            if (isCharging)
            {
                chargeTimer += Time.deltaTime;

                if (promptText != null)
                {
                    float progress = (chargeTimer / chargeTime) * 100f;
                    promptText.gameObject.SetActive(true);
                    promptText.text = "A carregar... " + Mathf.RoundToInt(progress) + "%";
                }

                if (chargeTimer >= chargeTime)
                {
                    FinishCharging();
                }
            }
        }

        // 2. Se eu afastei-me, e era eu que estava a controlar a UI, então largo o controlo.
        if (!isNear && activeCharger == this)
        {
            CancelCharging();
            
            if (promptText != null)
                promptText.gameObject.SetActive(false);
                
            activeCharger = null; // Liberta a UI para outras tomadas usarem
        }
    }

    void StartCharging()
    {
        isCharging = true;
        chargeTimer = 0f;
    }

    void FinishCharging()
    {
        BatteryManager.Instance.AddBattery(chargeAmount);
        isCharging = false;
        onCooldown = true;
        cooldownTimer = cooldown;

        // Correção do Renderer
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null) rend.material.color = Color.gray;

        // Quando termina de carregar, esconde o texto e liberta a UI
        if (promptText != null)
        {
            promptText.text = "[E]Interagir";
            promptText.gameObject.SetActive(false);
        }
        activeCharger = null; 
    }

    void CancelCharging()
    {
        isCharging = false;
        chargeTimer = 0f;

        if (promptText != null)
            promptText.text = "[E]Interagir";
    }
}