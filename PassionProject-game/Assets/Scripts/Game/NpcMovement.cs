using System;
using NUnit.Framework.Internal;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;


public class NpcMovement : MonoBehaviour
{

    public GameObject ball;
    private Vector3 target;


    private Ballcontroller ballController;


    public float moveSpeed = 3f;
    public float stopDistance = 1.2f;


    private Animator animator;
    private bool ballInRange = false;

    public Transform hitPoint;

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

    void Start()
    {
        animator = GetComponent<Animator>();
        ballController = ball.GetComponent<Ballcontroller>();
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
            animator.SetFloat("Direction", 0); // idle
            return;
        }

        Vector3 dir = moveDirection.normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        // Smooth rotation
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 8f
        );

        animator.SetFloat("Direction", 1);
    
    }

    private void DetermineSwingTrigger()
    {
        Vector3 localBallPos = transform.InverseTransformPoint(target);

        Vector3 dirToBall = target - transform.position;
        dirToBall.y = 0; // keep horizontal
        if (dirToBall.sqrMagnitude < 0.001f) dirToBall = transform.forward;

        // ball = high
        if (target.y > hitPoint.position.y + 0.5f)
        {
            swingRotation = Quaternion.LookRotation(dirToBall) * Quaternion.Euler(0, 20f, 0); // slight turn to the right
            swingType = "Overhand";
        }
        else if (localBallPos.x > 0)
        {
            swingRotation = Quaternion.LookRotation(dirToBall) * Quaternion.Euler(0, 33f, 0);
            swingType = "Forehand";
        }
        else if (localBallPos.x < 0)
        {
            swingRotation = Quaternion.LookRotation(dirToBall) * Quaternion.Euler(0, -33f, 0);
            swingType = "Backhand";
        }
        else
        {
            swingRotation = Quaternion.LookRotation(dirToBall); // just face the ball
            swingType = "Forehand";    // pick a reasonable default
        }
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
        target = ball.transform.position;

        if (!isSwinging)
        {
            Move();
            MoveTowardBall();
        }

        if (ballInRange && !isSwinging)
        {
            TrySwing();
        }

        if (isSwinging)
        {
            RotateSwing();
        }
    }
}