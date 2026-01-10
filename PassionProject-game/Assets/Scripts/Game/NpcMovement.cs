using System;
using NUnit.Framework.Internal;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;


public class NpcMovement : MonoBehaviour
{

    public GameObject ball;
    private Vector3 target;

    public Ballcontroller ballController;
    public NpcHitsystem NpcHitScript;

    public float moveSpeed = 3f;
    public float stopDistance = 1.2f;


    private Animator animator;
    private bool ballInRange = false;

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
    private bool isMoving = false;
    public Ballcontroller.CourtZone lastBallZone;

    [Header("Ball Prediction")]
    private Vector3 predictedLandingPoint;
    public bool hasPrediction = false;
    public bool predictionLocked = false;
    public float zPredictionOffset = 0.8f;


    private float TimeToGroundBounce(Rigidbody rb)
    {
        float y = rb.position.y;
        float vy = rb.linearVelocity.y;
        float g = Physics.gravity.y;

        // Solve: y + vy*t + 0.5*g*t^2 = groundHeight (≈0)
        float discriminant = vy * vy - 2f * g * y;

        if (discriminant < 0f) return -1f;

        return (-vy - Mathf.Sqrt(discriminant)) / g;
    }
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
    }

    private bool IsPredictionInMyZone()
    {
        return hasPrediction && ballController.currentZone == myZone;
    }

    void UpdatePrediction()
    {
        Rigidbody rb = ballController.rb; // Only predict if ball is moving toward my side 

        bool ballComingToMe = (side == CourtSide.Left && ballController.rightSide) || (side == CourtSide.Right && ballController.leftSide);
        if (!ballComingToMe)
        {
            hasPrediction = false;
            return;
        }

        //Only predict if ball is in the air and moving 

        if (rb.linearVelocity.magnitude < 0.1f && rb.transform.position.y > 0.5f)
        {
            hasPrediction = false;
            return;
        }

        // Simple forward prediction 
        float predictionTime = 0.6f; // tweak later 
        predictedLandingPoint = ball.transform.position + rb.linearVelocity * predictionTime; // Keep on ground 
        predictedLandingPoint.y = transform.position.y;

        // Clamp to my court side 
        if (side == CourtSide.Left)
        {
            predictedLandingPoint.z = Mathf.Min(predictedLandingPoint.z, 0f);
            predictedLandingPoint.z += zPredictionOffset;
        }
        else
        {
            predictedLandingPoint.z = Mathf.Max(predictedLandingPoint.z, 0f);
            predictedLandingPoint.z -= zPredictionOffset;
        }

        hasPrediction = true;

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
                timerTreshold = UnityEngine.Random.Range(1.5f, 2.2f);
            }
            else
            {
                // Ball is on the other side → shorter move
                timerTreshold = UnityEngine.Random.Range(0.8f, 1.1f);
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


        // transform.rotation = Quaternion.Slerp(
        //     transform.rotation,
        //     Quaternion.LookRotation(dir),
        //     Time.deltaTime * 8f
        // );
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
        {
            direction = (horizontalMovement > 0f) ? 1 : (horizontalMovement < 0f ? -1 : 0);
        }
        else // left side
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


    // Update is called once per frame
    void Update()
    {
        //target = ball.transform.position;
        UpdatePrediction();

        // bool ballOnMySide =
        //     (side == CourtSide.Left && ballController.leftSide) ||
        //     (side == CourtSide.Right && ballController.rightSide);

        // if (ballOnMySide && !predictionLocked || !hasPrediction)
        // {
        //     target = ball.transform.position;
        // }
        // else
        // {
        //     target = predictedLandingPoint;
        // }

        bool ballOnMySide =
            (side == CourtSide.Left && ballController.leftSide) ||
            (side == CourtSide.Right && ballController.rightSide);

        // Use predictedLandingPoint only if ball is still coming
        if (!ballOnMySide && IsPredictionInMyZone())
        {
            target = predictedLandingPoint; // go to predicted strike spot
        }
        else
        {
            target = ball.transform.position; // ball is on my side → chase normally
            predictionLocked = false;          // unlock for next shot
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

        if (isSwinging)
        {
            RotateSwing();
        }
    }
}

