using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    [Header("References")]
    public Animator playerAnimation;
    //public EspUdp espManager;
    public int playerId = 1;  // 1 or 2, set in Inspector

    public GameManager gameManager;
    public EspUdp espManager;

    [Header("Swing Detection")]
    [SerializeField] private float swingThreshold = 7f;
    public bool swingActive = false;
    public float timer;
    public float swingEnergy;
    private bool collecting;
    private float windowTimer;
    private float sumAx, sumAy, sumAz;
    public float peakAx, peakAy, peakAz;
    public float peakGx, peakGy, peakGz;
    public float peakAxPos, peakAxNeg;

    [Header("cooldown")]
    [SerializeField] private float swingCooldown = 1f;
    private float cooldownTimer = 0f;
    private bool inCooldown = false;

    public enum SwingType { Forehand, Backhand, Overhand }
    public SwingType swingType = SwingType.Forehand;

    void Update()
    {
        if (gameManager.transitionMultiverse)
        {
            swingActive = false;
            return; 
        } 

        HandleCooldown();
        DetectSwing();
    }

    void DetectSwing()
    {
        if (inCooldown) return;
        EspUdp.EspData espData = (playerId == 1) ? espManager.esp1 : espManager.esp2;

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
            peakAx = peakAy = peakAz = 0f;
            peakGx = peakGy = peakGz = 0f;
            peakAxPos = 0f;
            peakAxNeg = 0f;
        }

        // collect motion
        if (collecting)
        {
            windowTimer += Time.deltaTime;

            if(windowTimer >= 0.080f)
            {
                sumAx += Mathf.Abs(ax);
                sumAy += Mathf.Abs(ay);
                sumAz += Mathf.Abs(az);

                peakAx = Mathf.Max(peakAx, ax);
                peakAy = Mathf.Max(peakAy, Mathf.Abs(ay));
                peakAz = Mathf.Max(peakAz, Mathf.Abs(az));

                peakGx = Mathf.Max(peakGx, gx);
                peakGy = Mathf.Max(peakGy, Mathf.Abs(gy));
                peakGz = Mathf.Max(peakGz, Mathf.Abs(gz));

                if (ax > peakAxPos) peakAxPos = ax;
                if (ax < peakAxNeg) peakAxNeg = ax;
            }

            if (windowTimer >= 0.350f) // 400ms window
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

        float total = sumAx + sumAy + sumAz;
        float absSumAx = sumAx;
        float absSumAz = sumAz;

        if(total < 0.001f)
        {
            swingActive = false;
            return;
        }

        float verticalRatio = absSumAz / total;
        float horizontalRatio = absSumAx / total; 
        float totalAccel = total;

        // ---------- THRESHOLDS (tune-friendly) ----------
        const float overhandVerticalRatio = 0.375f;  // vertical default 0.4
       // const float overhandMinAccel = 300f;    // strong motion

        const float forehandPeakAxMin = 10f;     // forehand = pos
        const float backhandPeakAxMax = -5.5f;    // backhand = neg


        // ---------- 1) OVERHAND DETECTION ----------
        // Overhand tends to have strong vertical accel and decent energy.
        if (verticalRatio > overhandVerticalRatio && peakAx > 5f)
        {
            playerAnimation.SetTrigger("Overhand");
            swingType = SwingType.Overhand;
            Debug.Log($"OVERHAND | vr={verticalRatio:F2}, totalAccel={totalAccel:F1} peakAx={peakAx:F1}, hr={horizontalRatio:F2}");
            return;
        }

        // ---------- 2) FOREHAND vs BACKHAND ----------
        // peak AX is either positive or negative / small
        if (peakAxPos > forehandPeakAxMin && Mathf.Abs(peakAxPos) > Mathf.Abs(peakAxNeg))
        {
            playerAnimation.SetTrigger("Forehand");
            swingType = SwingType.Forehand;
            Debug.Log($"FOREHAND | vr={verticalRatio:F2}, totalAccel={totalAccel:F1} , peakAx={peakAx:F1}, hr={horizontalRatio:F2}  ");
        }
        else if (peakAxNeg < backhandPeakAxMax && Mathf.Abs(peakAxNeg) > Mathf.Abs(peakAxPos))
        {
            playerAnimation.SetTrigger("Backhand");
            swingType = SwingType.Backhand;
            Debug.Log($"BACKHAND | vr={verticalRatio:F2}, totalAccel={totalAccel:F1}, peakAx={peakAx:F1}, hr={horizontalRatio:F2} ");
        }
        else
        {
            // default, almost never occurs though that this happens
            playerAnimation.SetTrigger("Forehand");
            swingType = SwingType.Forehand;
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
