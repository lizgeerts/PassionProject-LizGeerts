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
    }

    // Update is called once per frame
    void Update()
    {
        if (hitSensor.BallInHitCircle)
        {
            Debug.Log("Ball in hit range");
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

            Transform activeHitPoint = GetActiveHitPoint();

            Vector3 shotDir = activeHitPoint.forward;
            shotDir += Vector3.up * upFactor;
            shotDir.Normalize();

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(shotDir * hitForce, ForceMode.VelocityChange);
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


    public void OpenHitWindow()
    {
        racketCollider.enabled = false;
        hitWindowOpen = true;
        hasHitThisSwing = false;
        hasLaunched = false;
        timer = 0f;
    }

    public void CloseHitWindow()
    {
        hitWindowOpen = false;
        racketCollider.enabled = true;
        timer = 0f;
    }
}
