using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public BallLaunch ballLaunchScript;
    //public EspUdp espData;
    public Animator playerAnimation;
    //public EspConnect espData; => if using via cable not wifi
    public BallHit2 ballHitScript;
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

    void Update()
    {
        // Vector3 move = Vector3.zero;

        // int dir = espData.joystickDir;
        // isMoving = false;

        // switch (dir)
        // {
        //     case 1: move = transform.forward; isMoving = true; break;
        //     case 2: move = -transform.forward; isMoving = true; break;
        //     case 3:
        //         {
        //             move = transform.right;
        //             lastRunLeft = false;
        //             isMoving = true;
        //             break;
        //         }
        //     case 4:
        //         {
        //             move = -transform.right;
        //             lastRunLeft = true;
        //             isMoving = true;
        //             break;
        //         }
        // }

        // animationDir = 0f;
        // if (isMoving)
        // {
        //     animationDir = lastRunLeft ? 1 : -1;
        // }
        // RotatePlayer();

        // playerAnimation.SetFloat("Direction", animationDir);

        // controller.Move(move * moveSpeed * Time.deltaTime);
        // Vector3 pos = transform.position;
        // pos.y = 0.842f; //keep on the floor
        // transform.position = pos;
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
        else if (ballHitScript.mySide == BallHit2.CourtSide.Left)
        {//if overhand, only x offset to the side the ball is
            xOffset = 0.3f;
        }
        else xOffset = -0.3f;

        //add z offset = behind the ball
        if (ballHitScript.mySide == BallHit2.CourtSide.Left)
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




    // private void RotatePlayer()
    // {
    //     //rotate player when moving
    //     if (isMoving)
    //     {
    //         if (animationDir == 1)
    //         {
    //             transform.rotation = Quaternion.Euler(0, -15, 0);
    //         }
    //         else if (animationDir == -1)
    //         {
    //             transform.rotation = Quaternion.Euler(0, 15, 0);
    //         }
    //     }
    //     else
    //     {
    //         transform.rotation = Quaternion.Euler(0, 0, 0);
    //     }
    // }
}
