using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    public Animator playerAnimation;
    public EspUdp espData;

    public float swingThreshold = 7f;   // tune this
    public float resetThreshold = 4f;
    public bool swingActive = false;
    public float swingEnergy;
    private float rotation;

    void Update()
    {
        DetectSwing();
        RotatePlayer();
    }

    void DetectSwing()
    {
        float gx = espData.gx;
        float gy = espData.gy;
        float gz = espData.gz;
        float ax = espData.ax;
        float ay = espData.ay;
        float az = espData.az;

        swingEnergy = Mathf.Abs(gx) + Mathf.Abs(gy) + Mathf.Abs(gz);
        // Debug.Log(swingEnergy);

        if (!swingActive && swingEnergy > swingThreshold)
        {
            swingActive = true;
            TriggerSwing(ax, ay, az, gx, gy, gz);
        }

        // end of swing : (reset)
        if (swingActive && swingEnergy < resetThreshold)
        {
            swingActive = false;
            rotation = 0f;
        }
    }

    void TriggerSwing(float ax, float ay, float az, float gx, float gy, float gz)
    {
        playerAnimation.ResetTrigger("Forehand");
        playerAnimation.ResetTrigger("Backhand");
        playerAnimation.ResetTrigger("Overhand");


        // PRIORITY 2: FOREHAND (moderate ay with positive gy)
        if (ay >= -8 && ay < 3 && gy > -2)
        {
            playerAnimation.SetTrigger("Forehand");
            Debug.Log($"forehand (ay:{ay:F2} gy:{gy:F2})");
            rotation = 45f;
            return;
        }

        if (ay > -7)
        {
            playerAnimation.SetTrigger("Backhand");
            Debug.Log($"backhand (ay:{ay:F2})");
            rotation = -45f;
            return;
        }

        // PRIORITY 3: OVERHAND (wild motion)
        if (ax > 10 && (Mathf.Abs(gy) > 3 || ay > 2))
        {
            playerAnimation.SetTrigger("Overhand");
            Debug.Log($"overhand (ax:{ax:F2} ay:{ay:F2})");
            rotation = 35f;
            return;
        }
    }

    private void RotatePlayer()
    {
        //rotate player when swinging
        transform.rotation = Quaternion.Euler(0, rotation, 0);
    }
}

