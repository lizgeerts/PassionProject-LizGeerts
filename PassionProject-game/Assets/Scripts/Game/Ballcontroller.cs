using NUnit.Framework;
using UnityEngine;

public class Ballcontroller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Rigidbody rb; //get the balls rigidbody
    public float serveSpeed = 5;
    public float hitForce = 5;
    public Vector3 lastHitDirection;

    public bool leftSide = false;
    public bool rightSide = false;

    public int bounceCount = 0;
    public NpcHitsystem NpcHitScript;

    public enum CourtZone
    {
        Box1,
        Box2,
        Box3,
        Box4
    }

    public CourtZone currentZone;

    public bool hasServed { get; private set; } = false;
    public void RegisterFirstHit()
    {
        if (hasServed) return;

        hasServed = true;
        Debug.Log("FIRST HIT OF RALLY");
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
        else if (collision.collider.CompareTag("Racket"))
        {
            Debug.Log("Ball hit racket");
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

    }

    // Update is called once per frame
    void Update()
    {


    }
}
