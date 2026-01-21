using System;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    public Animator playerAnimation;
    public EspUdp espData;

    private float swingThreshold;
    private float resetThreshold;
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

        if (Math.Abs(ax) > 7) //overhand or forehand
        {
            swingThreshold = 8f;
            resetThreshold = 4f;
        }
        else if (Math.Abs(ay) > 7) //overhand
        {
            swingThreshold = 4f;
            resetThreshold = 2f;
        }

        swingEnergy = Mathf.Abs(gx) + Mathf.Abs(gy) + Mathf.Abs(gz);
        // Debug.Log(swingEnergy);
        //Debug.Log($"Thres: {swingThreshold}");


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


        // forehand ax = ± 10, ay = ± 2, az = ± 0.5
        if (ax >= 8)
        {
            playerAnimation.SetTrigger("Forehand");
            Debug.Log($"forehand (ay:{ay:F2} gy:{gy:F2})");
            rotation = 45f;
            return;
        }

        // backhand ax = ± -10, ay = ± 2, az = ± 0.5
        if (ax <= -8)
        {
            playerAnimation.SetTrigger("Backhand");
            Debug.Log($"backhand (ay:{ay:F2})");
            rotation = -45f;
            return;
        }

        // overhand ax = ± 1, ay = ± 10, az = ± 1
        if (ay >= 8)
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
        // transform.rotation = Quaternion.Euler(0, rotation, 0);
      //  Debug.Log("rotation");
    }
}

