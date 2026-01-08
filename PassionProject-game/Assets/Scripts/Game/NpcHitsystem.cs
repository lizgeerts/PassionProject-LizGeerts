using UnityEngine;

public class NpcHitsystem : MonoBehaviour
{

    public HitSensor hitSensor;
    public bool hitWindowOpen;

    private bool hasHitThisSwing;

    public Ballcontroller ball;
    public NpcMovement NPCscript;
    public Transform hitPoint;

    public Transform GetActiveHitPoint()
    {
        if (NPCscript.swingType == "Backhand")
            return hitPoint.Find("HitPoint-back");
        else
            return hitPoint.Find("HitPoint-front");
    }

    Vector3 ComputeShotDirection()
    {
        // Forward relative to NPC
        Vector3 dir = GetActiveHitPoint().forward;

        // Add lift so the ball always goes up
        dir += Vector3.up * 0.4f;

        return dir.normalized;
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
            Debug.Log(NPCscript.swingType);
        }

        if (hitWindowOpen && hitSensor.BallInHitCircle && !hasHitThisSwing)
        {
            PerformHit();
        }
    }

    void PerformHit()
    {
        hasHitThisSwing = true;
        Debug.Log("HIT!");

        Rigidbody rb = ball.rb;

        Transform activeHitPoint = GetActiveHitPoint();

        ball.transform.position = Vector3.Lerp(ball.transform.position, activeHitPoint.position, 0.5f);

        // Reset velocity for consistency
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 shotDir = ComputeShotDirection();

        float hitForce = 7f; // tune later
        rb.AddForce(shotDir * hitForce, ForceMode.VelocityChange);

    }

    public void OpenHitWindow()
    {
        hitWindowOpen = true;
        hasHitThisSwing = false;
    }

    public void CloseHitWindow()
    {
        hitWindowOpen = false;
    }
}
