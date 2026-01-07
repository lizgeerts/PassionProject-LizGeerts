using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class NPCcontroller : MonoBehaviour
{

    public GameObject ball;

    public float moveSpeed = 3f;
    public float stopDistance = 1.2f;

    private Animator animator;
    private bool ballInRange = false;

    public enum CourtSide { Left, Right }
    public CourtSide side;

    public float swingCooldown = 1f;
    private float lastSwingTime = 0f;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void MoveTowardBall()
    {

        Vector3 target = ball.transform.position;
        target.y = transform.position.y;

        float distance = Vector3.Distance(transform.position, target);

        if (side == CourtSide.Left)
        {
            if (target.z > transform.position.z)
            {
                target.z = transform.position.z;
            }
        }
        else
        {
            if (target.z < transform.position.z)
            {
                target.z = transform.position.z;
            }
        }

        // float leftBound = -1.5f; 
        // float rightBound = 1.5f;
        // target.x = Mathf.Clamp(target.x, leftBound, rightBound);


        if (distance > stopDistance)
        {
            bool ballOnMySide = (side == CourtSide.Left && ball.GetComponent<Ballcontroller>().leftSide) ||
                    (side == CourtSide.Right && ball.GetComponent<Ballcontroller>().rightSide);

            if (!ballOnMySide)
            {
                target.z = transform.position.z; 
            }

            Vector3 dir = (target - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;

            animator.SetFloat("Direction", 1);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 8f);
        }
        else
        {
            animator.SetFloat("Direction", 0);
        }
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

        MoveTowardBall();

        if (ballInRange)
        {
            TrySwing();
        }
    }
}
