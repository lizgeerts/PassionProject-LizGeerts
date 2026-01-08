using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;


public class NPCcontroller : MonoBehaviour
{


    public GameObject ball;
    private Vector3 target;


    private Ballcontroller ballController;


    public float moveSpeed = 3f;
    public float stopDistance = 1.2f;


    private Animator animator;
    private bool ballInRange = false;


    public enum CourtSide { Left, Right }
    public CourtSide side;


    public float swingCooldown = 1f;
    private float lastSwingTime = 0f;


    public Transform teammate;
    public float teamFollowWeight = 0.3f;


    private bool canMoveZ;
    private bool isClosest;
    private Vector3 moveDirection;

    public Transform hitPoint;

    [Header("Swinging")]
    private Quaternion swingRotation;
    private string swingType;
    private Quaternion preSwingRotation; // store NPC rotation before swing
    private bool isSwinging = false;
    private enum SwingPhase { None, ToSwing, Back }
    private SwingPhase swingPhase = SwingPhase.None;

    [Header("Zone Movement")]
    public Ballcontroller.CourtZone myZone;
    private bool ballInMyZone;


    void Start()
    {
        animator = GetComponent<Animator>();
        ballController = ball.GetComponent<Ballcontroller>();
    }


    void Move()
    {
        if (myZone == ballController.currentZone)
        {
            ballInMyZone = true;
        }
        else
        {
            ballInMyZone = false;
        }

        moveDirection = Vector3.zero;
        target.y = transform.position.y;

        if (ballInMyZone)
        {
            moveDirection = target - transform.position;
        }

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
            swingRotation = Quaternion.LookRotation(dirToBall) * Quaternion.Euler(0, 45f, 0);
            swingType = "Forehand";
        }
        else if (localBallPos.x < 0)
        {
            swingRotation = Quaternion.LookRotation(dirToBall) * Quaternion.Euler(0, -45f, 0);
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


        Move();


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