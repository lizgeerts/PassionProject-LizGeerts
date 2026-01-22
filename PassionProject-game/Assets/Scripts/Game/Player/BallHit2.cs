using UnityEngine;

public class BallHit2 : MonoBehaviour
{

    public EspUdp espData;
    public Rigidbody ballRigidbody;
    public Transform ball;
    public Collider playerCollider;
    public PlayerHit playerHitScript;
    public BallLaunch ballLaunchScript;

    public bool swingActive;
    public float swingEnergy;

    [Header("Ball Physics")]
    public Vector3 baseLaunchDirection = new Vector3(0, 0.3f, 1f);  // Forward + slight arc
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public enum CourtSide { Left, Right }
    public CourtSide mySide;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        swingActive = playerHitScript.swingActive;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && swingActive)
        //if the ball is in the capsule collider of the player and the player is swinging 
        //then launch the ball
        {
            HitBall();
        }
    }


    private void HitBall()
    {
        ballLaunchScript.isItPlayerSwinging = true;
        ballLaunchScript.state = BallLaunch.BallState.Hit;
    }
}
