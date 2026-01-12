using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NpcHitsystem : MonoBehaviour
{

    public HitSensor hitSensor;
    public bool hitWindowOpen = false;
    public bool animationStarted = false;

    private bool hasHitThisSwing;

    public Ballcontroller ball;
    public NpcMovement NPCscript;
    public Transform hitPoint;

    public Collider racketCollider;

    private float timer = 0f;
    public float timerTreshold = 50f;
    public float hitForce = 5;
    public float upFactor = 0.3f;
    public bool hasLaunched = false;
    private bool canLaunch = false;

    [Header("Other NPCs")]
    //public Transform targetNPC;
    public Transform[] targetNPCs;
    public int randomTarget;
    // public enum CourtSide { left, right }
    public NpcMovement.CourtSide swingSide;

    public Transform GetActiveHitPoint()
    {
        if (NPCscript.swingType == "Backhand")
            return hitPoint.Find("HitPoint-back");
        else if (NPCscript.swingType == "Overhand")
        {
            return hitPoint.Find("HitPoint-up");
        }
        else
        {
            return hitPoint.Find("HitPoint-front");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (hitSensor.BallInHitCircle)
        {
            Debug.Log("Ball in hit range");
            NPCscript.hasPrediction = false;
        }

        if (animationStarted && hitSensor.BallInHitCircle)
        {
            PerformHit();
        }

        if (ball.bounceCount == 2 && hitSensor.BallInHitCircle && ball.hasServed)
        {
            PerformHit();
        }
        //if the ball has bounced 2 times and at the same time is in range and
        //serving was done, then also perform a hit

        if (canLaunch)
        {
            FlyUp();
        }
    }

    void PerformHit()
    {
        Rigidbody rb = ball.rb;

        Transform activeHitPoint = GetActiveHitPoint();
        ball.transform.position = activeHitPoint.position;

        if (!hasHitThisSwing && hitWindowOpen)
        {
            swingSide = NPCscript.side;

            ball.RegisterFirstHit(); //set has served to true
            ball.bounceCount = 0;
            Debug.Log("HIT!");
            canLaunch = true;
            hasHitThisSwing = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private Transform chooseTarget()
    {
        randomTarget = UnityEngine.Random.Range(0, 2);
        Transform newTarget = targetNPCs[randomTarget];

        return newTarget;
    }
    void FlyUp()
    {
        timer += 1;

        if (timer >= timerTreshold && !hasLaunched)
        {
            Debug.Log("launched");
            Rigidbody rb = ball.rb;
            Transform activeHitPoint = GetActiveHitPoint();

            // horizontal direction toward target NPC
            Vector3 targetPos = chooseTarget().position;
            Vector3 horizontalDir = targetPos - activeHitPoint.position;
            horizontalDir.y = 0f;
            horizontalDir.Normalize();

            // horizontal speed = swing hardness
            if (NPCscript.swingType == "Backhand")
            {
                hitForce = 5;
                upFactor = 7;
            }
            else { hitForce = 5; upFactor = 8; }

            Vector3 velocity = horizontalDir * hitForce;

            // Vertical speed = arc height 
            velocity.y = upFactor;

            // launch
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = velocity;

            Debug.Log($"Launch velocity = {rb.linearVelocity}");

            canLaunch = false;
            hasLaunched = true;
        }
    }

    // void FlyUp()
    // {
    //     timer += 1;

    //     if (timer >= timerTreshold && !hasLaunched)
    //     {
    //         Debug.Log("launched");
    //         Rigidbody rb = ball.rb;
    //         Transform activeHitPoint = GetActiveHitPoint();

    //         // horizontal direction toward target NPC
    //         Vector3 targetPos = targetNPC.position;
    //         Vector3 horizontalDir = targetPos - activeHitPoint.position;
    //         horizontalDir.y = 0f;
    //         horizontalDir.Normalize();

    //         // horizontal speed = swing hardness
    //         Vector3 velocity = horizontalDir * hitForce;

    //         // Vertical speed = arc height 
    //         velocity.y = upFactor;

    //         // launch
    //         rb.linearVelocity = Vector3.zero;
    //         rb.angularVelocity = Vector3.zero;
    //         rb.linearVelocity = velocity;

    //         Debug.Log($"Launch velocity = {rb.linearVelocity}");

    //         canLaunch = false;
    //         hasLaunched = true;
    //     }
    // }



    public void startSwing()
    {
        racketCollider.enabled = false;
        animationStarted = true;
    }
    public void OpenHitWindow()
    {
        hitWindowOpen = true;
        hasHitThisSwing = false;
        hasLaunched = false;
        timer = 0f;
    }

    public void CloseHitWindow()
    {
        animationStarted = false;
        hitWindowOpen = false;
        racketCollider.enabled = true;
        timer = 0f;
    }
}
