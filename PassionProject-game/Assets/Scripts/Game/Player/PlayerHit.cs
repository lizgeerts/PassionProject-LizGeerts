using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    public Animator playerAnimation;
    public EspConnect espData;

    [Header("Swing Thresholds")]
    public float minPower = 3;  // Power ≥5 = swing!

    [Header("Forehand")]
    public float forehandMinPitch = 20f;
    public float forehandMaxPitch = 65f;
    public float forehandMinRoll = -10f;
    public float forehandMaxRoll = 50f;

    [Header("Backhand")]
    public float backhandMinPitch = 25f;
    public float backhandMaxPitch = 55f;
    public float backhandMinRoll = 25f;
    public float backhandMaxRoll = 55f;

    [Header("Overhand")]
    public float overhandMaxPitch = 20f;  
    public float overhandMinRoll = 40f;
    public float overhandMaxRoll = 80f;

    // Cooldown (prevent spam)
    float lastSwingTime = 0f;
    public float swingCooldown = 1f;

    void Start()
    {

    }

    void Update()
    {   // Power check + cooldown
        if (espData.power < minPower ||
            Time.time - lastSwingTime < swingCooldown)
            return;

        // FOREHAND: High pitch + wide roll range
        if (espData.pitch >= forehandMinPitch && espData.pitch <= forehandMaxPitch &&
            espData.roll >= forehandMinRoll && espData.roll <= forehandMaxRoll)
        {
            playerAnimation.SetTrigger("Forehand");
            lastSwingTime = Time.time;
            //  Debug.Log($"FOREHAND! P:{espData.pitch:F1} R:{espData.roll:F1} Power:{espData.power}");
            return;
        }

        // BACKHAND: Medium pitch + roll swing
        if (espData.pitch >= backhandMinPitch && espData.pitch <= backhandMaxPitch &&
            espData.roll >= backhandMinRoll && espData.roll <= backhandMaxRoll)
        {
            playerAnimation.SetTrigger("Backhand");
            lastSwingTime = Time.time;
            //Debug.Log($"BACKHAND! P:{espData.pitch:F1} R:{espData.roll:F1} Power:{espData.power}");
            return;
        }

        // OVERHAND: Low/neg pitch + high roll
        if (espData.pitch <= overhandMaxPitch &&
            espData.roll >= overhandMinRoll && espData.roll <= overhandMaxRoll)
        {
            playerAnimation.SetTrigger("Overhand");
            lastSwingTime = Time.time;
            // Debug.Log($"OVERHAND! P:{espData.pitch:F1} R:{espData.roll:F1} Power:{espData.power}");
            return;
        }
    }

}
