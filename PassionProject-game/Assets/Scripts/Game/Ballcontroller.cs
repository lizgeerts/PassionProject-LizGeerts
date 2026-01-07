using UnityEngine;

public class Ballcontroller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Rigidbody rb; //get the balls rigidbody
    public float serveSpeed = 5;
    public float hitForce = 5;
    public Transform leftSide; //transform is component of GObject that stores position, rotation etc..
    public Transform rightSide;
    public Vector3 lastHitDirection;
    public bool inPlay = false;


    public void Serve(Vector3 direction)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(direction.normalized * serveSpeed, ForceMode.VelocityChange);
        inPlay = true;
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
            Debug.Log("Ball hit glass");

        }
    }



    void HitByRacket(Collision collision)
    {
        // Direction from racket to ball
        Vector3 hitDir = (transform.position - collision.transform.position).normalized;

        // Apply force
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(hitDir * hitForce, ForceMode.VelocityChange);

        lastHitDirection = hitDir;
    }


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
