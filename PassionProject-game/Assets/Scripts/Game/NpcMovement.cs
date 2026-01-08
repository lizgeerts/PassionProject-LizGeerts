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
    private float swingRotation; 
    private string swingType;
    private Quaternion preSwingRotation; // store NPC rotation before swing
    private bool isSwinging = false;

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

        // ball = high
        if (target.y > hitPoint.position.y + 0.4f)
        {
            swingRotation = 20f; // slight turn to the right
            swingType = "Overhand";
        } else if (localBallPos.x > 0){
            swingRotation = 90f;
            swingType = "Forehand";
        } else if (localBallPos.x < 0)
        {
            swingRotation = -90f;
            swingType = "Backhand";
        }
    }

    void TrySwing()
    {

        if (Time.time - lastSwingTime < swingCooldown) return;

        preSwingRotation = transform.rotation; //store rotation
        DetermineSwingTrigger();

        Vector3 dirToBall = target - transform.position;
        dirToBall.y = 0;
        if (dirToBall.sqrMagnitude < 0.001f) dirToBall = transform.forward;
        float targetY = Mathf.Atan2(dirToBall.x, dirToBall.z) * Mathf.Rad2Deg + swingRotation;
        Quaternion targetRot = Quaternion.Euler(0, targetY, 0);

        transform.rotation = targetRot;


        animator.SetTrigger(swingType);

        lastSwingTime = Time.time;
        isSwinging = true;
    }

    private Quaternion ComputeSwingRotation()
    {
        // Direction to ball on horizontal plane
        Vector3 dirToBall = target - transform.position;
        dirToBall.y = 0;

        if (dirToBall.sqrMagnitude < 0.001f) dirToBall = transform.forward;

        Quaternion lookRot = Quaternion.LookRotation(dirToBall);
        Quaternion swingOffset = Quaternion.Euler(0, swingRotation, 0); // swingRotation from DetermineSwingTrigger

        return lookRot * swingOffset;
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
            DetermineSwingTrigger();
            TrySwing();
        }

        if (isSwinging)
        {
            // only rotate yaw back to pre-swing rotation
            float currentY = transform.eulerAngles.y;
            float targetY = preSwingRotation.eulerAngles.y;
            float newY = Mathf.LerpAngle(currentY, targetY, Time.deltaTime * 4f);

            transform.rotation = Quaternion.Euler(0, newY, 0);

            if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, preSwingRotation.eulerAngles.y)) < 0.5f)
                isSwinging = false;
        }
    }
}