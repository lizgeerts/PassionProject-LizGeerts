using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    [Header("References")]
    public Animator playerAnimation;
    public EspUdp espData;

    [Header("Swing Detection")]
    private float swingThreshold = 7f;
    public bool swingActive = false;
    public float timer;
    public float swingEnergy;
    private bool collecting;
    private float windowTimer;
    private float sumAx, sumAy, sumAz;
    float peakAx, peakAy, peakAz;

    [Header("cooldown")]
    private float swingCooldown = 0.75f;
    private float cooldownTimer = 0f;
    private bool inCooldown = false;

    public enum SwingType { Forehand, Backhand, Overhand }
    public SwingType swingType = SwingType.Forehand;
    private Quaternion swingRotation;

    void Update()
    {
        HandleCooldown();
        DetectSwing();
        RotateWithSwing();

        //Debug.Log($"phase: {swingPhase} swinging:{playerIsSwinging} swingtype:{swingType}");
        // Debug.Log(swingActive);
    }

    void DetectSwing()
    {
        if (inCooldown) return;

        float gx = espData.gx;
        float gy = espData.gy;
        float gz = espData.gz;

        float ax = espData.ax;
        float ay = espData.ay;
        float az = espData.az;

        swingEnergy = Mathf.Abs(gx) + Mathf.Abs(gy) + Mathf.Abs(gz);

        // start collecting
        if (!collecting && swingEnergy > swingThreshold)
        {
            swingActive = true;
            collecting = true;
            windowTimer = 0f;
            sumAx = sumAy = sumAz = 0f;
        }

        // collect motion
        if (collecting)
        {
            windowTimer += Time.deltaTime;

            sumAx += Mathf.Abs(ax);
            sumAy += Mathf.Abs(ay);
            sumAz += Mathf.Abs(az);

            peakAx = Mathf.Max(peakAx, ax);
            peakAy = Mathf.Max(peakAy, Mathf.Abs(ay));
            peakAz = Mathf.Max(peakAz, Mathf.Abs(az));

            if (windowTimer >= 0.400f) // 400ms window
            {
                ClassifySwing();
                collecting = false;
                inCooldown = true;
                cooldownTimer = swingCooldown;
                peakAx = peakAy = peakAz = 0f;
            }
        }
    }

    void ClassifySwing()
    {
        playerAnimation.ResetTrigger("Forehand");
        playerAnimation.ResetTrigger("Backhand");
        playerAnimation.ResetTrigger("Overhand");

        //  Debug.Log($"AX:{sumAx:F1} AY:{sumAy:F1} AZ:{sumAz:F1}");
        Debug.Log($"Peak AX:{peakAx:F1} AY:{peakAy:F1} AZ:{peakAz:F1}");
        float total = sumAx + sumAy + sumAz;

        float axR = sumAx / total;
        float ayR = sumAy / total;
        float azR = sumAz / total;

        // OVERHAND: vertical dominance
        if (azR > 0.40f && peakAx < 17.3f)
        {
            playerAnimation.SetTrigger("Overhand");
            Debug.Log("OVERHAND");
            swingType = SwingType.Overhand;
            SetRotation();
            return;
        }

        if (sumAx > sumAy * 0.85f)
        {
            if (peakAx > 4f)
            {
                playerAnimation.SetTrigger("Forehand");
                Debug.Log("FOREHAND");
                swingType = SwingType.Forehand;
            }
            else if (peakAx < 1.5f)
            {
                playerAnimation.SetTrigger("Backhand");
                Debug.Log("BACKHAND");
                swingType = SwingType.Backhand;
            }
            SetRotation();
        }
    }

    void SetRotation()
    {
        switch (swingType)
        {
            case SwingType.Forehand:
                swingRotation = Quaternion.Euler(0f, 30f, 0f);
                break;

            case SwingType.Backhand:
                swingRotation = Quaternion.Euler(0f, -30f, 0f);
                break;

            case SwingType.Overhand:
                swingRotation = Quaternion.Euler(0f, 25f, 0f);
                break;
        }
    }

    void HandleCooldown()
    {
        if (!inCooldown) return;

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            swingRotation = Quaternion.Euler(0f, 0f, 0f);
            inCooldown = false;
            swingActive = false;
        }
    }

    void RotateWithSwing()
    {
        // transform.rotation = swingRotation;
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, swingRotation, Time.deltaTime * 500f);
    }

}
