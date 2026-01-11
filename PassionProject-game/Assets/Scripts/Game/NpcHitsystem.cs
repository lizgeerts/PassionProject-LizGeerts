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
    public Transform targetNPC;
    public enum CourtSide { left, right }
    public CourtSide swingSide;

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

        // if(ball.bounceCount == 2 && NPCscript.ballInRange && ball.hasServed)
        // {
        //     PerformHit();
        // }
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
            swingSide = (activeHitPoint.position.z >= 10f)
                ? CourtSide.left
                : CourtSide.right;

            ball.RegisterFirstHit(); //set has served to true
            ball.bounceCount = 0;
            Debug.Log("HIT!");
            canLaunch = true;
            hasHitThisSwing = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }


    void FlyUp()
    {
        timer += 1;
        // Debug.Log(timer);

        if (timer >= timerTreshold && !hasLaunched)
        {
            Debug.Log("launched");
            Rigidbody rb = ball.rb;

            Transform activeHitPoint = GetActiveHitPoint();
            // Vector3 targetPos = targetNPC.position;
            // targetPos.y += 5f;
            // Vector3 shotDir = activeHitPoint.forward;
            // shotDir += Vector3.up * upFactor;
            // shotDir.Normalize();

            Vector3 forward = activeHitPoint.forward;
            Vector3 targetPos = targetNPC.position;
            targetPos.y += 1000f;
            Vector3 toTarget = targetPos - activeHitPoint.position;
            toTarget.y = 0f;
            toTarget.Normalize();

            float aimBias = 0.8f; // 0 = pure forward, 1 = pure target
            Vector3 shotDir = Vector3.Lerp(forward, toTarget, aimBias);

            shotDir += Vector3.up * upFactor;
           // shotDir.Normalize();


            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = shotDir * hitForce;
            //rb.AddForce(shotDir * hitForce, ForceMode.VelocityChange);
            // Transform activeHitPoint = GetActiveHitPoint();

            // Vector3 dir = (ball.transform.position - activeHitPoint.position).normalized;
            // dir += Vector3.up * upFactor;

            // Vector3 shotDir = dir.normalized;

            // rb.linearVelocity = Vector3.zero;
            // rb.AddForce(shotDir * hitForce, ForceMode.VelocityChange);


            canLaunch = false;
            hasLaunched = true;
        }
    }




    // void FlyUp()
    // {
    //     timer += 1;

    //     if (timer >= timerTreshold && !hasLaunched)
    //     {
    //         Rigidbody rb = ball.rb;
    //         Transform activeHitPoint = GetActiveHitPoint();

    //         // Determine which NPC to aim at
    //         Vector3 targetPosition = targetNPC.position;

    //         // Compute direction from hit point to target NPC
    //         Vector3 shotDir = targetPosition - activeHitPoint.position;

    //         // Add a vertical component for the arc

    //         shotDir.y += upFactor;

    //         shotDir.Normalize();

    //         // Reset velocities and apply the launch
    //         rb.linearVelocity = Vector3.zero;
    //         rb.angularVelocity = Vector3.zero;
    //         rb.linearVelocity = shotDir * hitForce;

    //         canLaunch = false;
    //         hasLaunched = true;
    //     }
    // }


    // void FlyUp()
    // {
    //     timer += 1;

    //     if (timer >= timerTreshold && !hasLaunched)
    //     {
    //         Rigidbody rb = ball.rb;
    //         Transform activeHitPoint = GetActiveHitPoint();

    //         // Determine which NPC to aim at
    //         Vector3 targetPosition = targetNPC.position;

    //         // Compute direction from hit point to target NPC
    //         Vector3 shotDir = targetPosition ;

    //         // Add a vertical component for the arc

    //         shotDir.y += upFactor;

    //         shotDir.Normalize();

    //         // Reset velocities and apply the launch
    //         rb.linearVelocity = Vector3.zero;
    //         rb.angularVelocity = Vector3.zero;
    //         rb.linearVelocity = shotDir * hitForce;

    //         canLaunch = false;
    //         hasLaunched = true;
    //     }
    // }

    // void FlyUp()
    // {
    //     timer += 1;

    //     if (timer >= timerTreshold && !hasLaunched)
    //     {
    //         Rigidbody rb = ball.rb;
    //         Transform hit = GetActiveHitPoint();

    //         // 1️⃣ Horizontal direction toward target NPC
    //         Vector3 targetPos = targetNPC.position;
    //         targetPos.y += 1f;
    //         Vector3 horizontalDir = targetPos - hit.position;
    //         horizontalDir.y = 0f;
    //         horizontalDir.Normalize();

    //         // 2️⃣ Horizontal velocity
    //         float horizontalSpeed = hitForce;
    //         Vector3 velocity = horizontalDir * horizontalSpeed;

    //         // 3️⃣ Vertical velocity = ARC CONTROL
    //         velocity.y = hitForce * upFactor;

    //         // 4️⃣ Apply clean velocity
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
