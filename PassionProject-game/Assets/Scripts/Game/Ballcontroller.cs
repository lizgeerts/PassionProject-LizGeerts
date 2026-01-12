using NUnit.Framework;
using UnityEngine;

public class Ballcontroller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Rigidbody rb; //get the balls rigidbody
    public float serveSpeed = 5;
    public float hitForce = 5;
    public Vector3 lastHitDirection;
    public Vector3 homePos;

    public bool leftSide = false;
    public bool rightSide = false;

    public int bounceCount = 0;
    public NpcHitsystem NpcHitScript;


    public Transform leftNpc;
    public Transform rightNpc;
    public float zoneAssistStrength = 1.5f;

    public enum CourtZone
    {
        Box1,
        Box2,
        Box3,
        Box4
    }

    public CourtZone currentZone;

    [Header("NPC References")]
    public Transform NPC1;
    public Transform NPC2;
    public Transform NPC3;
    public Transform NPC4;


    [Header("Ball Nudge Settings")]
    public float nudgeStrength = 1.5f; // tweak this
    //private bool hasNudgedThisZone = false;

    public bool hasServed { get; set; } = false;


    public void RegisterFirstHit()
    {
        if (hasServed) return;

        hasServed = true;
    }

    public void Serve(Vector3 direction)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(direction.normalized * serveSpeed, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            // Debug.Log(hasServed);
            // Debug.Log("Ball hit floor");
            if (hasServed)
            {
                bounceCount++;
                if (bounceCount == 1)
                {
                    ApplyFirstBounceDamping();
                }
            }
            lastHitDirection = rb.linearVelocity.normalized;
        }
        else if (collision.collider.CompareTag("Cage"))
        {
            Debug.Log("Ball hit cage");
        }
        else if (collision.collider.CompareTag("Glass"))
        {
            Debug.Log("Ball hit glass");

        }
    }

    private void ApplyFirstBounceDamping()
    {
        Vector3 v = rb.linearVelocity;

        v.y *= 0.7f;   //take slight bounce off

        rb.linearVelocity = v;
    }

    void OnTriggerEnter(Collider other)
    {
        //which side
        if (other.CompareTag("Left"))
            leftSide = true;
        if (other.CompareTag("Right"))
            rightSide = true;

        //which box
        BoxScript box = other.GetComponent<BoxScript>();
        if (box != null)
        {
            currentZone = box.zone;
            // Debug.Log("Ball entered zone: " + currentZone);
           // hasNudgedThisZone = false;
            if (hasServed)
            {
              //  NudgeTowardNPC();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Left"))
            leftSide = false;
        if (other.CompareTag("Right"))
            rightSide = false;
    }


    void Start()
    {
        homePos = transform.position;
    }

    // Update is called once per frame

    // private void NudgeTowardNPC()
    // {
    //     if (hasNudgedThisZone) return;


    //     Transform targetNPC = null;

    //     // Decide which NPC should get the nudge
    //     switch (currentZone)
    //     {
    //         case CourtZone.Box1: targetNPC = NPC1; break;
    //         case CourtZone.Box2: targetNPC = NPC2; break;
    //         case CourtZone.Box3: targetNPC = NPC3; break;
    //         case CourtZone.Box4: targetNPC = NPC4; break;
    //     }

    //     if (targetNPC == null) return;


    //     // bool ballOnLeft = transform.position.z >= 10f;
    //     bool ballOnLeft = leftSide;

    //     // Where the swing started:
    //     bool swingWasLeft = (NpcHitScript.swingSide == NpcMovement.CourtSide.Left);
    //     //bool swingWasLeft = (NpcHitScript.swingSide == NpcHitsystem.CourtSide.left);

    //     // We only nudge if ball is now on the opposite side from where it was hit
    //     bool isOppositeSide =
    //         (swingWasLeft && !ballOnLeft) ||
    //         (!swingWasLeft && ballOnLeft);

    //     if (!isOppositeSide)
    //     {
    //         // Same side as swing → no assist nudge in these boxes
    //         return;
    //     }


    //     Vector3 dir = (targetNPC.position - transform.position).normalized;

    //     // Only nudge in x and z, keep y velocity intact
    //     Vector3 newVelocity = rb.linearVelocity;
    //     newVelocity.x += dir.x * nudgeStrength;
    //     newVelocity.z += dir.z * nudgeStrength;
    //     rb.linearVelocity = newVelocity;

    //     hasNudgedThisZone = true; // only once per zone entry
    //     Debug.Log("nudging towards" + targetNPC);
    // }


    void Update()
    {


    }
}
