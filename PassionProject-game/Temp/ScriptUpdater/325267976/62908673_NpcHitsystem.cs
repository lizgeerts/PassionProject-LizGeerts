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
            ball.RegisterFirstHit(); //set has served to true
            ball.bounceCount = 0;
            Debug.Log("HIT!");
            canLaunch = true;
            hasHitThisSwing = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }


    // void FlyUp()
    // {
    //     timer += 1;
    //     // Debug.Log(timer);

    //     if (timer >= timerTreshold && !hasLaunched)
    //     {
    //         Debug.Log("launched");
    //         Rigidbody rb = ball.rb;

    //         Transform activeHitPoint = GetActiveHitPoint();

    //         Vector3 shotDir = activeHitPoint.forward;
    //         shotDir += Vector3.up * upFactor;
    //         shotDir.Normalize();

    //         rb.linearVelocity = Vector3.zero;
    //         rb.angularVelocity = Vector3.zero;
    //         rb.linearVelocity = shotDir * hitForce;
    //         //rb.AddForce(shotDir * hitForce, ForceMode.VelocityChange);
    //         // Transform activeHitPoint = GetActiveHitPoint();

    //         // Vector3 dir = (ball.transform.position - activeHitPoint.position).normalized;
    //         // dir += Vector3.up * upFactor;

    //         // Vector3 shotDir = dir.normalized;

    //         // rb.linearVelocity = Vector3.zero;
    //         // rb.AddForce(shotDir * hitForce, ForceMode.VelocityChange);


    //         canLaunch = false;
    //         hasLaunched = true;
    //     }
    // }

    void FlyUp()
    {
        timer += 1;

        if (timer >= timerTreshold && !hasLaunched)
        {
            Rigidbody rb = ball.rb;
            Transform activeHitPoint = GetActiveHitPoint();

            Vector3 shotDir;

            // Predefined directions per swing type (relative to court)
            switch (NPCscript.swingType)
            {
                case "Forehand":
                    shotDir = NPCscript.side == NpcMovement.CourtSide.Left
                        ? new Vector3(0, 0, 1)  // always forward to right court
                        : new Vector3(0, 0, -1); // forward to left court
                    break;

                case "Backhand":
                    shotDir = NPCscript.side == NpcMovement.CourtSide.Left
                        ? new Vector3(0, 0, 1)
                        : new Vector3(0, 0, -1);
                    break;

                case "Overhand":
                    shotDir = NPCscript.side == NpcMovement.CourtSide.Left
                        ? new Vector3(0, 0, 1)
                        : new Vector3(0, 0, -1);
                    break;

                default:
                    shotDir = NPCscript.side == NpcMovement.CourtSide.Left
                        ? new Vector3(0, 0, 1)
                        : new Vector3(0, 0, -1);
                    break;
            }

            // Add consistent upward arc
            shotDir += Vector3.up * upFactor;
            shotDir.Normalize();

            // Apply velocity
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = shotDir * hitForce;

            canLaunch = false;
            hasLaunched = true;
        }
    }



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
