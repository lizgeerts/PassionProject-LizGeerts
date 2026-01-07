using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;


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
    public float minX; //get them to stay in their lane
    public float maxX;



    void Start()
    {
        animator = GetComponent<Animator>();
        ballController = ball.GetComponent<Ballcontroller>();
    }


    void MoveZ()
    {
        //is the ball on my side of the court
        bool ballOnMySide = (side == CourtSide.Left && ballController.leftSide) ||
        (side == CourtSide.Right && ballController.rightSide);


        //is the ball behind me
        bool ballBehindMe =
        (side == CourtSide.Left && target.z < transform.position.z) ||
        (side == CourtSide.Right && target.z > transform.position.z);


        canMoveZ = ballOnMySide && !ballBehindMe;
    }


    void ClosestOne()
    {
        float distance = Vector3.Distance(transform.position, target); //distance to ball
        float teammateDist = Vector3.Distance(teammate.position, target); //teammates distance

        isClosest = distance <= teammateDist; //is this npc closer
    }


    void ChooseMovement()
    {
        moveDirection = Vector3.zero;
        target.y = transform.position.y; //keep npc from flying
        target.x = Mathf.Clamp(target.x, minX, maxX);


        if (canMoveZ && isClosest) //then you can fully move
        {
            moveDirection = target - transform.position;
        }
        else //otherwise only sideways
        {
            float xDiff = target.x - transform.position.x;
            if (Math.Abs(xDiff) > 0.2f)
            {
                moveDirection = new Vector3(xDiff, 0, 0);
            }
        }
    }


    void MoveTowardBall()
    {

        if (moveDirection.magnitude < 0.05f)
        {
            animator.SetFloat("Direction", 0);
            return;
        }


        Vector3 dir = moveDirection.normalized;

        transform.position += dir * moveSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Slerp(
        transform.rotation,
        Quaternion.LookRotation(dir),
        Time.deltaTime * 8f
        );


        animator.SetFloat("Direction", 1);
    }


    void TrySwing()
    {


        if (Time.time - lastSwingTime < swingCooldown) return;


        if (ballInRange)
        {
            animator.SetTrigger("Forehand");
            lastSwingTime = Time.time;
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


        MoveZ();
        ClosestOne();
        ChooseMovement();
        MoveTowardBall();


        if (ballInRange)
        {
            TrySwing();
        }
    }
}