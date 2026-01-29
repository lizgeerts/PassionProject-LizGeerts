using NUnit.Framework;
using UnityEngine;

public class PlayerBallHit : MonoBehaviour
{
    public PlayerHit playerHitScript;
    public BallLaunch ballLaunchScript;

    public GameManager gameManager;

    public bool swingActive;
    public float swingEnergy;

    public enum CourtSide { Left, Right }
    public CourtSide mySide;
    public Vector3 startPosition;
    public Quaternion startRotation;
    public bool ballCanLaunch = false;

    [Header("Sounds")]
    [SerializeField] private AudioClip ballHitClip;
    public bool soundPlayed = false;

    void Update()
    {
        swingActive = playerHitScript.swingActive;
    }

    void OnTriggerStay(Collider other)
    {
        if (gameManager.transitionMultiverse) return; //no swinging during transition

        if (other.CompareTag("Ball") && swingActive)
        //if the ball is in the capsule collider of the player and the player is swinging 
        //then launch the ball
        {
            HitBall();
        }
    }


    private void HitBall()
    {
        if (ballCanLaunch)
        {
            playerHitScript.ConsumeSwing();

            if (!soundPlayed)
            {
                SoundFXManager.instance.PlaySoundFXClip(ballHitClip, transform, 1f, 0f);
                soundPlayed = true;
            }
            ballLaunchScript.isItPlayerSwinging = true;
            ballLaunchScript.state = BallLaunch.BallState.Hit;
        }
    }


    public void ResetToStart()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }

    public void OpenLaunchWindow()
    {
        ballCanLaunch = true;
    }
}
