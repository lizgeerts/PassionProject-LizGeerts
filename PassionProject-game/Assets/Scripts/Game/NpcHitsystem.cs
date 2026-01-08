using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NpcHitsystem : MonoBehaviour
{

    public HitSensor hitSensor;
    public bool hitWindowOpen;

    private bool hasHitThisSwing;

    public Ballcontroller ball;
    public NpcMovement NPCscript;
    public Transform hitPoint;

    public Collider racketCollider;

    private float timer = 0f;
    public float timerTreshold = 50f;
    public float hitForce = 5;
    public float upFactor = 0.3f;
    private bool hasLaunched = false;
    private bool canLaunch = false;

    public Transform GetActiveHitPoint()
    {
        if (NPCscript.swingType == "Backhand")
            return hitPoint.Find("HitPoint-back");
        else
            return hitPoint.Find("HitPoint-front");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        racketCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (hitSensor.BallInHitCircle)
        {
            // Debug.Log("Ball in hit range");
            // Debug.Log(NPCscript.swingType);
        }

        if (hitWindowOpen && hitSensor.BallInHitCircle)
        {
            PerformHit();
        }

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
       // ball.transform.position = Vector3.Lerp(ball.transform.position, activeHitPoint.position, 0.9f);

        // Reset velocity for consistency
        // rb.linearVelocity = Vector3.zero;
        // rb.angularVelocity = Vector3.zero;

        // Vector3 direction = (GetActiveHitPoint().position - ball.transform.position).normalized;
        // // Add lift so the ball always goes up
        // direction += Vector3.up * 0.4f;
        // Vector3 shotDir = direction.normalized;

        // float hitForce = 10f; // tune later
        // rb.AddForce(shotDir * hitForce, ForceMode.VelocityChange);

        if (!hasHitThisSwing)
        {
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
        Debug.Log(timer);
        if (timer >= timerTreshold && !hasLaunched)
        {
            Debug.Log("launched");
            Rigidbody rb = ball.rb;

            // Vector3 hitDir = (ball.transform.position - hitPoint.position).normalized + Vector3.up * 0.3f;
            // //transform = balls transform
            // // collision.transform = rackets transform
            // /* so this gives a vector from the racket → toward the ball. 
            //    if the ball is in front. the force goes forward
            //    +vector 3 gives it an upward lift = more real
            // */

            // hitDir.Normalize();

            // // Apply force
            // rb.linearVelocity = Vector3.zero;
            // rb.AddForce(hitDir * 5, ForceMode.VelocityChange);

            Transform activeHitPoint = GetActiveHitPoint();
            Vector3 hitDir = (ball.transform.position - activeHitPoint.position).normalized + Vector3.up * upFactor;

            hitDir.Normalize();

            rb.linearVelocity = Vector3.zero;
            rb.AddForce(hitDir * hitForce, ForceMode.VelocityChange);

            canLaunch = false;
            hasLaunched = true;
        }

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
        hitWindowOpen = false;
        racketCollider.enabled = false;
        timer = 0f;
    }
}
