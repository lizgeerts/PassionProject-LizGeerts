using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    public Animator playerAnimation;
    public EspUdp espData;

    public float swingThreshold = 7f;   // tune this
    public float resetThreshold = 4f;
    private bool swingActive = false;

    void Update()
    {
        DetectSwing();
    }

    void DetectSwing()
    {
        float gx = espData.gx;
        float gy = espData.gy;
        float gz = espData.gz;
        float ax = espData.ax;
        float ay = espData.ay;
        float az = espData.az;

        float swingEnergy = Mathf.Abs(gx) + Mathf.Abs(gy) + Mathf.Abs(gz);
       // Debug.Log(swingEnergy);

        if (!swingActive && swingEnergy > swingThreshold)
        {
            swingActive = true;
            TriggerSwing(ax, ay, az, gx, gy, gz);
        }

        // end of swing (reset)
        if (swingActive && swingEnergy < resetThreshold)
        {
            swingActive = false;
        }
    }

    void TriggerSwing(float ax, float ay, float az, float gx, float gy, float gz)
    {
        playerAnimation.ResetTrigger("Forehand");
        playerAnimation.ResetTrigger("Backhand");
        playerAnimation.ResetTrigger("Overhand");

        //  Debug.Log($"gy: {Mathf.Abs(gy)} gx:{Mathf.Abs(gx)}");
        // if (Mathf.Abs(gy) > Mathf.Abs(gx))
        // {
        //     if (gy > 0)
        //     {
        //         playerAnimation.SetTrigger("Forehand");
        //         Debug.Log("FOREHAND");
        //     }
        //     else
        //     {
        //         playerAnimation.SetTrigger("Backhand");
        //         Debug.Log("BACKHAND");
        //     }
        // } else
        // {
        //     playerAnimation.SetTrigger("Overhand");
        //     Debug.Log("OVERHAND");
        // }

        // PRIORITY 1: BACKHAND (ay < -7 is UNIQUE to backhand)
        if (ay < -7)
        {
            playerAnimation.SetTrigger("Backhand");
            Debug.Log($"BACKHAND (ay:{ay:F2})");
            return;
        }

        // PRIORITY 2: FOREHAND (moderate ay with positive gy)
        if (ay >= -8 && ay < 3 && gy > -2)
        {
            playerAnimation.SetTrigger("Forehand");
            Debug.Log($"FOREHAND (ay:{ay:F2} gy:{gy:F2})");
            return;
        }

        // PRIORITY 3: OVERHAND (wild motion)
        if (ax > 10 && (Mathf.Abs(gy) > 3 || ay > 2))
        {
            playerAnimation.SetTrigger("Overhand");
            Debug.Log($"OVERHAND (ax:{ax:F2} ay:{ay:F2})");
            return;
        }
    }
}
