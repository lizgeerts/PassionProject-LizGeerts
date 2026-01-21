// using System;
// using UnityEditorInternal;
// using UnityEngine;

// public class PlayerHit : MonoBehaviour
// {
//     public Animator playerAnimation;
//     public EspUdp espData;

//     private float swingThreshold = 7f;
//     private float resetThreshold;
//     public bool swingActive = false;
//     public bool isSwinging = false;
//     public float timer;
//     public float swingEnergy;
//     private float rotation;

//     void Update()
//     {
//         DetectSwing();
//         //  RotatePlayer();
//     }

//     void DetectSwing()
//     {
//         float gx = espData.gx;
//         float gy = espData.gy;
//         float gz = espData.gz;
//         float ax = espData.ax;
//         float ay = espData.ay;
//         float az = espData.az;

//         swingEnergy = Mathf.Abs(gx) + Mathf.Abs(gy) + Mathf.Abs(gz);
//         bool AXisBigger = ax > ay;
//         Debug.Log(AXisBigger);
//         //Debug.Log(swingEnergy);
//         //Debug.Log($"Thres: {swingThreshold}");

//         if (!swingActive && swingEnergy > swingThreshold)
//         {
//             swingActive = true;
//             TriggerSwing(ax, ay, az, gx, gy, gz);
//         }

//         if (swingActive)
//         {
//             timer += Time.deltaTime;
//         }

//         if (timer >= 0.8f) //cooldown 
//         {
//             swingActive = false;
//             timer = 0f;
//             rotation = 0f;
//         }

//         // Debug.Log(swingActive);

//         if(swingEnergy > swingThreshold)
//         {
//             TriggerSwing(ax, ay, az, gx, gy, gz);
//         }

//         // Debug.Log("Timer " + timer);

//         // end of swing : (reset)
//         // if (swingActive && swingEnergy < resetThreshold)
//         // {
//         //     swingActive = false;
//         //     rotation = 0f;
//         // }
//     }

//     void TriggerSwing(float ax, float ay, float az, float gx, float gy, float gz)
//     {

//         playerAnimation.ResetTrigger("Forehand");
//         playerAnimation.ResetTrigger("Backhand");
//         playerAnimation.ResetTrigger("Overhand");


//         // forehand ax = ± 10, ay = ± 2, az = ± 0.5
//         if (ax >= 9)
//         {
//             playerAnimation.SetTrigger("Forehand");
//             Debug.Log($"forehand (ay:{ay:F2} gy:{gy:F2})");
//             rotation = 45f;
//             return;
//         }

//         // backhand ax = ± -10, ay = ± 2, az = ± 0.5
//         if (ax <= -9)
//         {
//             playerAnimation.SetTrigger("Backhand");
//             Debug.Log($"backhand (ay:{ay:F2})");
//             rotation = -45f;
//             return;
//         }

//         // overhand ax = ± 1, ay = ± 10, az = ± 1
//         if (ay >= 6)
//         {
//             playerAnimation.SetTrigger("Overhand");
//             Debug.Log($"overhand (ax:{ax:F2} ay:{ay:F2})");
//             rotation = 35f;
//             return;
//         }
//     }

//     private void RotatePlayer()
//     {
//         //rotate player when swinging
//         //Quaternion targetRotation = Quaternion.Euler(0, rotation, 0);
//         // transform.rotation =  Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
//         transform.rotation = Quaternion.Euler(0, rotation, 0);
//     }

// }


// using System;
// using UnityEditorInternal;
// using UnityEngine;

// public class PlayerHit : MonoBehaviour
// {
//     public Animator playerAnimation;
//     public EspUdp espData;

//     private float swingThreshold = 7f;
//     private float resetThreshold;
//     public bool swingActive = false;
//     public bool isSwinging = false;
//     public float timer;
//     public float swingEnergy;
//     private float rotation;

//     void Update()
//     {
//         DetectSwing();
//         //  RotatePlayer();
//     }

//     void DetectSwing()
//     {
//         float gx = espData.gx * 10f;
//         float gy = espData.gy * 10f;
//         float gz = espData.gz * 10f;
//         float ax = espData.ax;
//         float ay = espData.ay;
//         float az = espData.az;

//         swingEnergy = Mathf.Abs(gx) + Mathf.Abs(gy) + Mathf.Abs(gz);
//         TriggerSwing(ax, ay, az, gx, gy, gz);

//     }

//     void TriggerSwing(float ax, float ay, float az, float gx, float gy, float gz)
//     {

//         playerAnimation.ResetTrigger("Forehand");
//         playerAnimation.ResetTrigger("Backhand");
//         playerAnimation.ResetTrigger("Overhand");

//         // if(Mathf.Abs(gx) + Mathf.Abs(gy) + Mathf.Abs(gz) < 10f)
//         // {
//         //     return;
//         // }

//         if (ay >= 6.5)
//         {
//             playerAnimation.SetTrigger("Overhand");
//             Debug.Log($"overhand (ax:{ax:F2} ay:{ay:F2} az:{az:F2})");
//             rotation = 35f;
//             return;
//         }

//         // if (Mathf.Abs(gx) + Mathf.Abs(gy) + Mathf.Abs(gz) < 40f)
//         // {
//         //     return;
//         // }

//         // forehand ax = ± 10, ay = ± 2, az = ± 0.5
//         if (ax >= 9)
//         {
//             playerAnimation.SetTrigger("Forehand");
//             Debug.Log($"forehand (ax:{ax:F2} ay:{ay:F2} az:{az:F2})");
//             rotation = 45f;
//             return;
//         }

//         // backhand ax = ± -10, ay = ± 2, az = ± 0.5
//         if (ax <= -9)
//         {
//             playerAnimation.SetTrigger("Backhand");
//             Debug.Log($"backhand (ax:{ax:F2} ay:{ay:F2} az:{az:F2})");
//             rotation = -45f;
//             return;
//         }

//         // overhand ax = ± 1, ay = ± 10, az = ± 1

//     }

//     private void RotatePlayer()
//     {
//         //rotate player when swinging
//         //Quaternion targetRotation = Quaternion.Euler(0, rotation, 0);
//         // transform.rotation =  Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
//         transform.rotation = Quaternion.Euler(0, rotation, 0);
//     }

// }


using System;
using UnityEditorInternal;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    public Animator playerAnimation;
    public EspUdp espData;

    private float swingThreshold = 7f;
    private float resetThreshold;
    public bool swingActive = false;
    public bool isSwinging = false;
    public float timer;
    public float swingEnergy;
    private float rotation;
    private bool collecting;
    private float windowTimer;
    private float sumAx, sumAy, sumAz;

    private float swingCooldown = 0.8f;

    private float cooldownTimer = 0f;
    private bool inCooldown = false;

    void Update()
    {
        HandleCooldown();
        DetectSwing();
        //  RotatePlayer();
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

            if (windowTimer >= 0.400f) // 500ms window
            {
                ClassifySwing();
                collecting = false;
                inCooldown = true;
                cooldownTimer = swingCooldown;
            }
        }
    }

    void ClassifySwing()
    {
        playerAnimation.ResetTrigger("Forehand");
        playerAnimation.ResetTrigger("Backhand");
        playerAnimation.ResetTrigger("Overhand");

        Debug.Log($"AX:{sumAx:F1} AY:{sumAy:F1} AZ:{sumAz:F1}");
        float total = sumAx + sumAy + sumAz;

        float axR = sumAx / total;
        float ayR = sumAy / total;
        float azR = sumAz / total;

        // OVERHAND: vertical dominance
        if (azR > 0.42f )
        {
            playerAnimation.SetTrigger("Overhand");
            Debug.Log("OVERHAND");
            rotation = 35f;
            return;
        }

        // FORE / BACK: horizontal dominance
        if (sumAx > sumAy * 0.85f)
        {
            if (espData.ax > 0)
            {
                playerAnimation.SetTrigger("Forehand");
                Debug.Log("FOREHAND");
                rotation = 45f;
            }
            else if (espData.ax < 0)
            {
                playerAnimation.SetTrigger("Backhand");
                Debug.Log("BACKHAND");
                rotation = -45f;
            }
        }
    }

    void HandleCooldown()
    {
        if (!inCooldown) return;

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            inCooldown = false;
        }
    }


    void TriggerSwing(float ax, float ay, float az, float gx, float gy, float gz)
    {

        playerAnimation.ResetTrigger("Forehand");
        playerAnimation.ResetTrigger("Backhand");
        playerAnimation.ResetTrigger("Overhand");

        // if(Mathf.Abs(gx) + Mathf.Abs(gy) + Mathf.Abs(gz) < 10f)
        // {
        //     return;
        // }

        if (ay >= 6.5)
        {
            playerAnimation.SetTrigger("Overhand");
            Debug.Log($"overhand (ax:{ax:F2} ay:{ay:F2} az:{az:F2})");
            rotation = 35f;
            return;
        }

        // if (Mathf.Abs(gx) + Mathf.Abs(gy) + Mathf.Abs(gz) < 40f)
        // {
        //     return;
        // }

        // forehand ax = ± 10, ay = ± 2, az = ± 0.5
        if (ax >= 9)
        {
            playerAnimation.SetTrigger("Forehand");
            Debug.Log($"forehand (ax:{ax:F2} ay:{ay:F2} az:{az:F2})");
            rotation = 45f;
            return;
        }

        // backhand ax = ± -10, ay = ± 2, az = ± 0.5
        if (ax <= -9)
        {
            playerAnimation.SetTrigger("Backhand");
            Debug.Log($"backhand (ax:{ax:F2} ay:{ay:F2} az:{az:F2})");
            rotation = -45f;
            return;
        }

        // overhand ax = ± 1, ay = ± 10, az = ± 1

    }

    private void RotatePlayer()
    {
        //rotate player when swinging
        //Quaternion targetRotation = Quaternion.Euler(0, rotation, 0);
        // transform.rotation =  Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

}
