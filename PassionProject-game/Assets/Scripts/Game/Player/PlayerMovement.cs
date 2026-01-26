using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public BallLaunch ballLaunchScript;
    //public EspUdp espData;
    public Animator playerAnimation;
    //public EspConnect espData; => if using via cable not wifi
    public PlayerBallHit ballHitScript;
    public float moveSpeed = 3f;
    public float stopDistance = 0.2f;

    private bool isBallComingToMe = false;
    public float moveTimer;
    private float xOffset = 0f;
    private float zOffset = 0f;
    private Vector3 targetHitPos;


    void Start()
    {

    }


    void FixedUpdate()
    {
        // Only react if the ball is coming to me
        if (ballLaunchScript.state == BallLaunch.BallState.Flying && ballLaunchScript.targetPlayer.name == gameObject.name ||
            ballLaunchScript.state == BallLaunch.BallState.Floating && ballLaunchScript.targetPlayer.name == gameObject.name)
        {
            if (!isBallComingToMe)
            {
                isBallComingToMe = true;
                CalculateTargetHitPos();
            }

            MoveToTarget();
        }
        else
        {
            // Reset
            isBallComingToMe = false;
            playerAnimation.SetFloat("Direction", 0f);
        }

    }

    void CalculateTargetHitPos()
    {
        Vector3 ballPoint = ballLaunchScript.hitPos;

        // Add small random offset so NPC is not perfectly on top
        if (ballLaunchScript.randomYOffset < 0.30f) //if not overhand
        {
            xOffset = Random.value > 0.3f ? -0.55f : 0.55f; //prefer forehand -> bigger chance
        }
        else if (ballHitScript.mySide == PlayerBallHit.CourtSide.Left)
        {//if overhand, only x offset to the side the ball is
            xOffset = 0.3f;
        }
        else xOffset = -0.3f;

        //add z offset = behind the ball
        if (ballHitScript.mySide == PlayerBallHit.CourtSide.Left)
        {
            zOffset = 0.3f;
        }
        else zOffset = -0.3f;


        targetHitPos = new Vector3(
            ballPoint.x + xOffset,
            0.82f, // stay on ground 
            ballPoint.z + zOffset
        );
    }

    void MoveToTarget()
    {

       // Vector3 move = Vector3.zero;

        Vector3 targetPos = targetHitPos;
        targetPos.y = transform.position.y; //extra lock for y

        Vector3 moveDir = targetPos - transform.position;
        float distance = moveDir.magnitude;

        if (distance > stopDistance)
        {
            Vector3 move = moveDir.normalized * moveSpeed * Time.fixedDeltaTime;
            controller.Move(move);

            // Animation direction
            float xDiff = targetHitPos.x - transform.position.x;
            float animDirection = 0f;

            if (Mathf.Abs(xDiff) > 0.1f)
            {
                animDirection = (xDiff > 0) ? 1f : -1f;
            }

            playerAnimation.SetFloat("Direction", animDirection);
        }
        else
        {
            playerAnimation.SetFloat("Direction", 0f);
        }

    }

}
