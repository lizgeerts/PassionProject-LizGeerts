using UnityEngine;

public class Ballcontroller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Rigidbody rb; //get the balls rigidbody
    public float serveSpeed = 5;
    public float hitForce = 5;
    // public Transform leftSide; //transform is component of GObject that stores position, rotation etc..
    // public Transform rightSide;
    public Vector3 lastHitDirection;
    // public bool inPlay = false;

    public bool leftSide = false;
    public bool rightSide = false;

    public enum CourtZone
    {
        Box1,
        Box2,
        Box3,
        Box4
    }

    public CourtZone currentZone;

    public void Serve(Vector3 direction)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(direction.normalized * serveSpeed, ForceMode.VelocityChange);
        // inPlay = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            Debug.Log("Ball hit floor");
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
            HitByRacket(collision);
        }
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
            Debug.Log("Ball entered zone: " + currentZone);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Left"))
            leftSide = false;
        if (other.CompareTag("Right"))
            rightSide = false;
    }

    void HitByRacket(Collision collision)
    {

        Vector3 hitDir = (transform.position - collision.transform.position).normalized + Vector3.up * 0.3f;
        //transform = balls transform
        // collision.transform = rackets transform
        /* so this gives a vector from the racket → toward the ball. 
           if the ball is in front. the force goes forward
           +vector 3 gives it an upward lift = more real
        */

        hitDir.Normalize();

        // Apply force
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(hitDir * hitForce, ForceMode.VelocityChange);

        lastHitDirection = hitDir; //for future
    }


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }
}
