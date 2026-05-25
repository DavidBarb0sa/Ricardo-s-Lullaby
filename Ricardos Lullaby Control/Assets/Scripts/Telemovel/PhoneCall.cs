using UnityEngine;

public class PhoneCall : MonoBehaviour
{
    [Header("Referências")]
    public Transform landlinePhone;
    public EnemyControl enemy;

    [Header("Chamada")]
    public float callDuration = 15f;
    [Header("Som")]
    public AudioSource phoneSound;

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

      
    if (phoneSound != null)
    {
        Debug.Log("A tocar som!");
        phoneSound.Play();
    }
    else
        Debug.Log("phoneSound é null!");  if (phoneSound != null)
        phoneSound.Play();
    }

    void EndCall()
    {
        callActive = false;
        enemy.ResumeChasing();

        if (phoneSound != null)
        phoneSound.Stop();
    }
}