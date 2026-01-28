using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    [Header("References")]
    public Animator playerAnimation;
    public EspUdp espData;
    public GameManager gameManager;

    [Header("Swing Detection")]
    private float swingThreshold = 7f;
    public bool swingActive = false;
    public float timer;
    public float swingEnergy;
    private bool collecting;
    private float windowTimer;
    private float sumAx, sumAy, sumAz;
    public float peakAx, peakAy, peakAz;
    public float peakGx, peakGy, peakGz;

    [Header("cooldown")]
    private float swingCooldown = 0.75f;
    private float cooldownTimer = 0f;
    private bool inCooldown = false;

    public enum SwingType { Forehand, Backhand, Overhand }
    public SwingType swingType = SwingType.Forehand;

    void Update()
    {
        if (gameManager.transitionMultiverse) return; //no swinging during transition

        HandleCooldown();
        DetectSwing();
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
       // Debug.Log("energy:"+ swingEnergy);

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

            peakGx = Mathf.Max(peakGx, gx);
            peakGy = Mathf.Max(peakGy, Mathf.Abs(gy));
            peakGz = Mathf.Max(peakGz, Mathf.Abs(gz));

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
       // Debug.Log($"Peak AX:{peakAx:F1} AY:{peakAy:F1} AZ:{peakAz:F1}");
        float total = sumAx + sumAy + sumAz;

        float azR = sumAz / total;

        // OVERHAND: vertical dominance
        if (azR > 0.40f && peakAx < 17.3f)
        {
            playerAnimation.SetTrigger("Overhand");
            Debug.Log("OVERHAND");
            swingType = SwingType.Overhand;
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
        } else
        {
            playerAnimation.SetTrigger("Forehand");
            Debug.Log("FOREHAND");
            swingType = SwingType.Forehand; //default is forehand
        }
    }


    void HandleCooldown()
    {
        if (!inCooldown) return;

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            inCooldown = false;
            swingActive = false;
        }
    }

    public void ConsumeSwing()
    {
        swingActive = false;
        collecting = false;
    }
}
