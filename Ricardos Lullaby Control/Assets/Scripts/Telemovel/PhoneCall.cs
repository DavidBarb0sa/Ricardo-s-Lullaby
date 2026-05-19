using UnityEngine;

public class PhoneCall : MonoBehaviour
{
    [Header("Referências")]
    public Transform landlinePhone;
    public EnemyControl enemy;

    [Header("Chamada")]
    public float callDuration = 15f;

    private bool callActive = false;
    private float callTimer = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && BatteryManager.Instance.HasBattery() && !callActive)
        {
            MakeCall();
        }

        if (callActive)
        {
            callTimer -= Time.deltaTime;
            if (callTimer <= 0f)
                EndCall();
        }
    }

    void MakeCall()
    {
        BatteryManager.Instance.UseBattery();
        callActive = true;
        callTimer = callDuration;
        enemy.GoToPosition(landlinePhone.position);
    }

    void EndCall()
    {
        callActive = false;
        enemy.ResumeChasing();
    }
}