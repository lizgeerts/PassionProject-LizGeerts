using System;
using UnityEngine;

public class BallHit : MonoBehaviour
{
    // public Transform racketCube;
    public EspUdp espData;
    public Rigidbody ballRigidbody;
    public Collider playerCollider;
    public PlayerHit playerHitScript;

    bool swingActive;
    float swingEnergy;

    [Header("Ball Physics")]
    public Vector3 baseLaunchDirection = new Vector3(0, 0.3f, 1f);  // Forward + slight arc


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        swingActive = playerHitScript.swingActive;
        swingEnergy = playerHitScript.swingEnergy;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") )
        //if the ball is in the capsule collider of the player and the player is swinging 
        //then launch the ball
        {
            HitBall();
        }
    }

    void HitBall()
    {
        // 1. base direction = over the net
        Vector3 launchDir = baseLaunchDirection.normalized;

        // 2. Power: controlled
        float power = Mathf.Clamp(swingEnergy * 2f, 2f, 8f);  

        // 3. Mpu influence: Subtle direction changes only
        Vector3 gyroInfluence = new Vector3(
            espData.gx * 0.05f,    // ±3° left/right max
            espData.gy * 0.1f,     // ±6° up/down max  
            0f                     // Always forward!
        );

        launchDir += gyroInfluence;
        launchDir.Normalize();

        // 4. velocity: Base speed + MPU influence
        Vector3 velocity = launchDir * power;

        // 5. controlled arc: Simple upward boost (no crazy gravity math)
        velocity.y = Mathf.Max(velocity.y, 3f);  
        velocity.y = Mathf.Min(velocity.y, 6f);  // not to high

        // 6. apply the force and velocity
        ballRigidbody.linearVelocity = velocity;
        ballRigidbody.angularVelocity = new Vector3(
            espData.gx * 2f,      // Side spin
            espData.gy * 4f,      // Top/back spin  
            0f
        );

        Debug.Log($"Ball hit! Power: {power:F1}, MPU: ({espData.gx:F2},{espData.gy:F2}), Vel: {velocity:F1}");
    }
}