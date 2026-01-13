using System;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;


public class NpcMovement : MonoBehaviour
{
    public GameObject ball;
    private Transform ballTransform;
    private Rigidbody ballRb;
    private Vector3 target;

    public Ballcontroller ballController;
    public NpcHitsystem NpcHitScript;
    public ResetGame ResetScript;

    public float moveSpeed = 3f;
    public float stopDistance = 1.2f;


    public Animator animator;
    public bool ballInRange = false;

    public Transform hitPoint;
    public Transform net;

    public enum CourtSide { Left, Right };
    public CourtSide side;


    [Header("Swinging")]
    private Quaternion swingRotation;
    public string swingType;
    private Quaternion preSwingRotation; // store NPC rotation before swing
    private bool isSwinging = false;
    private enum SwingPhase { None, ToSwing, Back }
    private SwingPhase swingPhase = SwingPhase.None;
    private float swingCooldown = 1f;
    private float lastSwingTime = 0f;

    [Header("Zone Movement")]
    public Ballcontroller.CourtZone myZone;
    private Vector3 moveDirection;
    private float timer = 0f;
    private float timerTreshold = 0f;
    public bool isMoving = false;
    public Ballcontroller.CourtZone lastBallZone;
    public float homeX;
    public Vector3 homePos;


    [Header("Ball Prediction")]
    private Vector3 predictedLandingPoint;
    public bool hasPrediction = false;
    public float zPredictionOffset = 0.8f;
    public bool predictionLock = false;

    [Header("Zone Limits")]
    public float maxZoneDepth = 1.2f;


    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    private Quaternion ClampRotationToCourt(Quaternion targetRotation)
    {
        Vector3 euler = targetRotation.eulerAngles;

        float clampRotation = NormalizeAngle(euler.y);

        if (side == CourtSide.Right)
        {
            // Right side: -90 to +90
            clampRotation = Mathf.Clamp(clampRotation, -60f, 60f);
        }
        else
        {
            // Left side: centered around 180
            clampRotation = Mathf.Clamp(clampRotation, 120, 240f);
        }

        return Quaternion.Euler(0f, clampRotation, 0f);
    }


    void Start()
    {
        animator = GetComponent<Animator>();
        ballController = ball.GetComponent<Ballcontroller>();
        ballTransform = ball.transform;
        ballRb = ball.GetComponent<Rigidbody>();
        homePos = transform.position; // store start x pos
        homeX = transform.position.x;
    }

    void UpdatePrediction()
    {
        if (predictionLock) return;
        // Only predict if ball is moving toward my side 

        bool ballComingToMe = (side == CourtSide.Left && NpcHitScript.swingSide == CourtSide.Right)
                               || (side == CourtSide.Right && NpcHitScript.swingSide == CourtSide.Left);
        if (!ballComingToMe)
        {
            //hasPrediction = false;
            return;
        }

        //Only predict if ball is moving 

        if (ballRb.linearVelocity.magnitude < 0.1f)
        {
            // hasPrediction = false;
            return;
        }

        // Simple forward prediction 
        float predictionTime = 0.6f; // tweak later 
        predictedLandingPoint = ballTransform.position + ballRb.linearVelocity * predictionTime; // Keep on ground 
        predictedLandingPoint.y = transform.position.y;

        // Clamp to my court side 
        if (side == CourtSide.Left)
        {
            predictedLandingPoint.z = Mathf.Min(predictedLandingPoint.z, 0f);
            predictedLandingPoint.z += zPredictionOffset; //add small offset, so they are behind the ball, not on it
        }
        else
        {
            predictedLandingPoint.z = Mathf.Max(predictedLandingPoint.z, 0f);
            predictedLandingPoint.z -= zPredictionOffset;
        }

        hasPrediction = true;
        predictionLock = true;
    }

    void Move()
    {
        target.y = transform.position.y; //keep from flying

        float distance = Vector3.Distance(transform.position, target); //distance to ball

        // Stop moving if the ball is close enough to swing
        if (distance <= 0.7f)
        {
            isMoving = false;
            moveDirection = Vector3.zero;
            animator.SetFloat("Direction", 0);
            return;
        }

        moveDirection = target - transform.position;
        moveDirection.y = 0;

        // Reset timer if ball has changed zones
        if (ballController.currentZone != lastBallZone)
        {
            timer = 0f;
            lastBallZone = ballController.currentZone;

            if ((side == CourtSide.Left && ballController.leftSide) ||
                (side == CourtSide.Right && ballController.rightSide))
            {
                // Ball is on my side → move a bit longer
                timerTreshold = UnityEngine.Random.Range(1.2f, 2f);
            }
            else
            {
                // Ball is on the other side → shorter move
                timerTreshold = UnityEngine.Random.Range(0.7f, 1f);
            }
        }

        if (myZone == ballController.currentZone)
        {
            // Ball is in my zone = fully moving
            isMoving = true;
        }
        else
        {
            // Ball is not in my zone = move only for short time
            timer += Time.deltaTime;
            isMoving = timer < timerTreshold;
        }
    }

    void MoveTowardBall()
    {
        if (isSwinging) return;

        if (!isMoving || moveDirection.magnitude < 0.05f)
        {

            Vector3 netDir = (net.position - transform.position);
            netDir.y = 0;

            transform.rotation = Quaternion.Slerp(
                 transform.rotation,
                 Quaternion.LookRotation(netDir),
                 Time.deltaTime * 8f
             );

            animator.SetFloat("Direction", 0); // idle
            return;
        }

        if (myZone != ballController.currentZone)
        {
            moveSpeed = 2.1f;
        }
        else { moveSpeed = 3.5f; }
        ;

        Vector3 dir = moveDirection.normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        Vector3 pos = transform.position;

        // clamp relative to starting position
        pos.x = Mathf.Clamp(
            pos.x,
            homeX - maxZoneDepth,
            homeX + maxZoneDepth
        );

        transform.position = pos;

        //good:
        Quaternion targetRot = Quaternion.LookRotation(dir);
        targetRot = ClampRotationToCourt(targetRot);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * 8f
        );

        // Determine animation direction based on X-axis movement
        float horizontalMovement = dir.x;

        int direction = 0;

        if (side == CourtSide.Left)
        //runleft = -1, runright = 1
        {
            direction = (horizontalMovement > 0f) ? -1 : (horizontalMovement < 0f ? 1 : 0);
        }
        else // right side
        {
            direction = (horizontalMovement < 0f) ? 1 : (horizontalMovement > 0f ? -1 : 0);
        }
        // Debug.Log(direction);

        animator.SetFloat("Direction", direction);
    }

    private void DetermineSwingTrigger()
    {
        Vector3 localBallPos = transform.InverseTransformPoint(target);
        Quaternion baseForward = (side == CourtSide.Left) ? Quaternion.Euler(0, 180f, 0) : Quaternion.Euler(0, 0f, 0);

        // Vector3 dirToBall = target - transform.position;
        // dirToBall.y = 0; // keep horizontal
        // if (dirToBall.sqrMagnitude < 0.001f) dirToBall = transform.forward;

        // // ball = high
        // if (target.y > hitPoint.position.y + 0.5f)
        // {
        //     swingRotation = Quaternion.LookRotation(dirToBall) * Quaternion.Euler(0, 17f, 0); // slight turn to the right
        //     swingType = "Overhand";
        // }
        // else if (localBallPos.x > 0)
        // {
        //     swingRotation = Quaternion.LookRotation(dirToBall) * Quaternion.Euler(0, 22f, 0);
        //     swingType = "Forehand";
        // }
        // else if (localBallPos.x < 0)
        // {
        //     swingRotation = Quaternion.LookRotation(dirToBall) * Quaternion.Euler(0, -45f, 0);
        //     swingType = "Backhand";
        // }
        // else
        // {
        //     swingRotation = Quaternion.LookRotation(dirToBall); // just face the ball
        //     swingType = "Forehand";    // pick a reasonable default
        // }

        Vector3 dirToBall = target - transform.position;
        dirToBall.y = 0; // keep horizontal
        if (dirToBall.sqrMagnitude < 0.001f) dirToBall = transform.forward;

        // Max swing angles relative to base forward
        float swingAngle = 0f;

        if (target.y > hitPoint.position.y + 0.5f)
        {
            swingAngle = 17f;  // Overhand
            swingType = "Overhand";
        }
        else if (localBallPos.x > 0)
        {
            swingAngle = 45f;  // Forehand
            swingType = "Forehand";
        }
        else if (localBallPos.x < 0)
        {
            swingAngle = -45f; // Backhand
            swingType = "Backhand";
        }
        else
        {
            swingAngle = 0f;
            swingType = "Forehand";
        }

        // Apply swing angle relative to base forward rotation
        swingRotation = baseForward * Quaternion.Euler(0f, swingAngle, 0f);
    }

    void TrySwing()
    {
        if (Time.time - lastSwingTime < swingCooldown) return;
        animator.SetFloat("Direction", 0f);
        preSwingRotation = transform.rotation; //store rotation
        DetermineSwingTrigger();

        animator.SetTrigger(swingType);
        lastSwingTime = Time.time;

        swingPhase = SwingPhase.ToSwing;
        isSwinging = true;
    }

    void RotateSwing()
    {
        switch (swingPhase)
        {
            case SwingPhase.ToSwing:
                transform.rotation = Quaternion.Slerp(transform.rotation, swingRotation, Time.deltaTime * 8f);
                if (Quaternion.Angle(transform.rotation, swingRotation) < 0.5f)
                {
                    swingPhase = SwingPhase.Back;
                }

                break;

            case SwingPhase.Back:
                transform.rotation = Quaternion.Slerp(transform.rotation, preSwingRotation, Time.deltaTime * 4f);

                // finished swinging
                if (Quaternion.Angle(transform.rotation, preSwingRotation) < 0.5f)
                {
                    isSwinging = false;
                    swingPhase = SwingPhase.None;
                }
                break;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            ballInRange = true;
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            ballInRange = false;
        }
    }

    private bool BallIsComingToMe()
    {
        return (side == CourtSide.Left && NpcHitScript.swingSide == CourtSide.Right)
            || (side == CourtSide.Right && NpcHitScript.swingSide == CourtSide.Left);
    }

    private bool BallOnMySide()
    {
        return (side == CourtSide.Left && ballController.leftSide)
            || (side == CourtSide.Right && ballController.rightSide);
    }
    // Update is called once per frame

    void Update()
    {
        if (isSwinging)
        {
            RotateSwing();
            return;
        }

        if (ballController.bounceCount >= 1)
        {
            hasPrediction = false;
            predictionLock = false;
        }

        UpdatePrediction();

        if (hasPrediction)
        {
            target = predictedLandingPoint;
        }
        else
        {
            target = ballTransform.position;
        }


        if (ballInRange && !isSwinging || ballController.bounceCount == 1 && myZone == ballController.currentZone)
        {
            TrySwing();
        }

        if (!isSwinging)
        {
            Move();
            MoveTowardBall();
        }
    }

    // void Update()
    // {

    //     UpdatePrediction();


    //     if (hasPrediction)
    //     {
    //         target = predictedLandingPoint;
    //     }
    //     else
    //     {
    //         target = ball.transform.position;
    //     }


    //     if (ballInRange && !isSwinging || ballController.bounceCount == 1 && myZone == ballController.currentZone)
    //     {
    //         TrySwing();
    //     }

    //     if (!isSwinging)
    //     {
    //         Move();
    //         MoveTowardBall();
    //     }

    //     if (isSwinging)
    //     {
    //         RotateSwing();
    //     }
    // }
}

