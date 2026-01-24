using UnityEngine;

public class BallHit2 : MonoBehaviour
{
    public EspUdp espData;
    public PlayerHit playerHitScript;
    public BallLaunch ballLaunchScript;

    public bool swingActive;
    public float swingEnergy;

    public enum CourtSide { Left, Right }
    public CourtSide mySide;
    public Vector3 startPosition;
    public Quaternion startRotation;

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


    public void ResetToStart()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}
