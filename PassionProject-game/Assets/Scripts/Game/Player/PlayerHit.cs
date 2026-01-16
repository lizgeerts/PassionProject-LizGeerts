using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    public Animator playerAnimation;
    public EspUdp espData;
    //public EspConnect espData; => if using via cable not wifi

    [Header("Swing Thresholds")]
 

    [Header("Forehand")]


    [Header("Backhand")]


    [Header("Overhand")]


    // Cooldown (prevent spam)
    float lastSwingTime = 0f;
    public float swingCooldown = 1f;

    void Start()
    {

    }

    void Update()
    {   // Power check + cooldown

    }

}
