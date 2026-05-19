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
    private bool hasBeenUsed = false;
    private float chargeTimer = 0f;

    private float cooldown = 30f;
    private float cooldownTimer = 0f;
    private bool onCooldown = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        if (promptText != null)
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
        GetComponent<Renderer>().material.color = Color.white; // volta à cor normal
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }
    return;
}

        float distance = Vector3.Distance(transform.position, player.position);
        bool isNear = distance <= interactDistance;

        // Mostra/esconde o prompt
        if (promptText != null)
            promptText.gameObject.SetActive(isNear && !isCharging);

        if (isNear && Input.GetKeyDown(KeyCode.E) && !isCharging)
        {
            StartCharging();
        }

        if (isCharging)
        {
            chargeTimer += Time.deltaTime;

            // Atualiza o prompt com progresso
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

            // Cancela se o jogador se afastar
            if (distance > interactDistance)
            {
                CancelCharging();
            }
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

    GetComponent<Renderer>().material.color = Color.gray;
}
    void CancelCharging()
    {
        isCharging = false;
        chargeTimer = 0f;

        if (promptText != null)
            promptText.text = "Prima E para carregar";
    }
    
}
