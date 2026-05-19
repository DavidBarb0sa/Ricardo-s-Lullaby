using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BatteryManager : MonoBehaviour
{
    public static BatteryManager Instance;

    [Header("Bateria")]
    public float battery = 0f;
    public float maxBattery = 100f;

    [Header("UI")]
    public TextMeshProUGUI batteryText;
    public Image batteryFill; // imagem de fill para a barra

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        UpdateUI();
    }

    public void AddBattery(float amount)
    {
        battery = Mathf.Clamp(battery + amount, 0f, maxBattery);
    }

    public bool HasBattery()
    {
         return battery >= maxBattery;
    }

    public void UseBattery()
    {
        battery = 0f;
    }

    void UpdateUI()
    {
        if (batteryText != null)
            batteryText.text = "Bateria: " + Mathf.RoundToInt(battery) + "%";

        if (batteryFill != null)
            batteryFill.fillAmount = battery / maxBattery;
    }
}
