using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    public Animator playerAnimation;
    public EspUdp espData;

    public float swingThreshold = 7f;   // tune this
    public float resetThreshold = 4f;
    public bool swingActive = false;
    public float swingEnergy;

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

        swingEnergy = Mathf.Abs(gx) + Mathf.Abs(gy) + Mathf.Abs(gz);
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


        // PRIORITY 2: FOREHAND (moderate ay with positive gy)
        if (ay >= -8 && ay < 3 && gy > -2)
        {
            playerAnimation.SetTrigger("Forehand");
            Debug.Log($"FOREHAND (ay:{ay:F2} gy:{gy:F2})");
            return;
        }

        if (ay > -7)
        {
            playerAnimation.SetTrigger("Backhand");
            Debug.Log($"BACKHAND (ay:{ay:F2})");
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


// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerHit : MonoBehaviour
// {
//     public Animator playerAnimation;
//     public EspUdp espData;

//     [Header("Swing Detection")]
//     public float swingThreshold = 7f;  
//     public float resetThreshold = 3.5f;      // lower than this = reset swing
//     public float swingWindow = 0.42f;      // seconds to record data
//     public float cooldownTime = 0.4f;      // min time between swings

//     private bool swingActive = false;
//     private float swingStartTime;
//     private float nextAllowedSwingTime = 0f;

//     // -------- IMU sample struct --------
//     struct ImuSample
//     {
//         public float ax, ay, az;
//         public float gx, gy, gz;
//     }

//     private List<ImuSample> samples = new List<ImuSample>();

//     void Update()
//     {
//         DetectSwing();
//     }

//     void DetectSwing()
//     {
//         if (Time.time < nextAllowedSwingTime)
//             return;
            
//         float gx = espData.gx;
//         float gy = espData.gy;
//         float gz = espData.gz;
//         float ax = espData.ax;
//         float ay = espData.ay;
//         float az = espData.az;

//         float swingEnergy = Mathf.Abs(gx) + Mathf.Abs(gy) + Mathf.Abs(gz);
//         // Debug.Log(swingEnergy);

//         if (!swingActive && swingEnergy > swingThreshold)
//         {
//             swingActive = true;
//             //TriggerSwing(ax, ay, az, gx, gy, gz);
//             swingStartTime = Time.time;
//             samples.Clear();
//         }

//         if (swingActive)
//         {
//             samples.Add(new ImuSample
//             {
//                 ax = ax,
//                 ay = ay,
//                 az = az,
//                 gx = gx,
//                 gy = gy,
//                 gz = gz
//             });

//             // ---- End of swing window ----
//             if (Time.time - swingStartTime >= swingWindow)
//             {
//                 ClassifySwing(samples);
//                 swingActive = false;
//                 nextAllowedSwingTime = Time.time + cooldownTime;
//             }
//         }

//         // ---- Reset if motion stops early ----
//         if (swingActive && swingEnergy < resetThreshold)
//         {
//             swingActive = false;
//             samples.Clear();
//         }
//     }

//     void ClassifySwing(List<ImuSample> samples)
//     {
//         if (samples.Count == 0)
//             return;

//         float peakGX = 0f, peakGY = 0f, peakGZ = 0f;
//         float avgAX = 0f, avgAY = 0f, avgAZ = 0f;

//         foreach (var s in samples)
//         {
//             peakGX = Mathf.Max(peakGX, Mathf.Abs(s.gx));
//             peakGY = Mathf.Max(peakGY, Mathf.Abs(s.gy));
//             peakGZ = Mathf.Max(peakGZ, Mathf.Abs(s.gz));

//             avgAX += s.ax;
//             avgAY += s.ay;
//             avgAZ += s.az;
//         }

//         avgAX /= samples.Count;
//         avgAY /= samples.Count;
//         avgAZ /= samples.Count;

//         // ---- Clear previous triggers ----
//         playerAnimation.ResetTrigger("Forehand");
//         playerAnimation.ResetTrigger("Backhand");
//         playerAnimation.ResetTrigger("Overhand");

//         // ================= CLASSIFICATION =================

//         // OVERHAND → upward accel + strong vertical rotation
//         if (avgAY > 2f && peakGY > peakGX)
//         {
//             playerAnimation.SetTrigger("Overhand");
//             Debug.Log("OVERHAND");
//             return;
//         }

//         // FOREHAND → sideways swing
//         if (avgAY < 0f && peakGY > peakGX)
//         {
//             playerAnimation.SetTrigger("Forehand");
//             Debug.Log("FOREHAND");
//             return;
//         }

//         // BACKHAND → strong negative Y accel + horizontal rotation
//         if (avgAY < -6f && peakGX > 3f)
//         {
//             playerAnimation.SetTrigger("Backhand");
//             Debug.Log("BACKHAND");
//             return;
//         }

//         Debug.Log("NO SWING TYPE MATCHED");
//     }

// }

