using UnityEngine;
using UnityEngine.UI;

public class BatterySliderBar : MonoBehaviour
{
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();

        if (BatteryManager.Instance != null && slider != null)
        {
            // FORÇAR A BATERIA A COMEÇAR NO MÁXIMO:
            BatteryManager.Instance.battery = BatteryManager.Instance.maxBattery;

            // Configura o Slider
            slider.minValue = 0f;
            slider.maxValue = BatteryManager.Instance.maxBattery;
            slider.value = BatteryManager.Instance.battery;
        }
    }

    void Update()
    {
        if (InventoryManager.Instance != null && InventoryManager.Instance.inventarioAberto)
            return;

        if (slider != null && BatteryManager.Instance != null)
            slider.value = BatteryManager.Instance.battery;
    }
}