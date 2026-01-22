using UnityEngine;

public class NpcHitBall : MonoBehaviour
{
    [Header("References")]
    public BallLaunch ballLaunchScript;
    public Animator animator;
    public Collider npcCollider;

    public enum CourtSide { Left, Right}
    public CourtSide mySide;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float stopDistance = 0.2f;

    [Header("Hit Settings")]
    public float forwardOffset = 0.3f; // Z offset toward/away from ball
    public float anticipation = 0.5f; // how far in advance NPC starts moving

    public Vector2 sideOffsetRange = new Vector2(-0.5f, 0.5f);
    public float reactionTime = 0.1f;


    private Vector3 targetHitPos;

    private bool isBallComingToMe = false;
    private bool hasHit = false;
    private float timer = 0f;
    public float moveTimer;


    void Start()
    {

    }


    void Update()
    {
        // Only react if the ball is coming to me
        if (ballLaunchScript.state == BallLaunch.BallState.Flying && ballLaunchScript.targetPlayer == transform ||
            ballLaunchScript.state == BallLaunch.BallState.Floating && ballLaunchScript.targetPlayer == transform)
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
            hasHit = false;
        }

        keepTrackAnimation();
    }


    void CalculateTargetHitPos()
    {
        // Base on ball's predicted hit position
        targetHitPos = ballLaunchScript.hitPos;

        // Add small random offset so NPC is not perfectly on top
        float xOffset = Random.Range(sideOffsetRange.x, sideOffsetRange.y);
        float zOffset = forwardOffset;

        targetHitPos += transform.right * xOffset + transform.forward * zOffset;
        targetHitPos.y = transform.position.y; // stay on ground
    }



    void MoveToTarget()
    {
        if (hasHit) return;

        // Move smoothly toward target
        Vector3 moveDir = targetHitPos - transform.position;
        moveDir.y = 0;
        float distance = moveDir.magnitude;

        if (distance > stopDistance)
        {
            transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("Ball"))
        {
            TriggerHitAnimation();
            hasHit = true;
        }
    }

    void TriggerHitAnimation()
    {
        Vector3 localBallPos = transform.InverseTransformPoint(ballLaunchScript.transform.position);

        animator.ResetTrigger("Forehand");
        animator.ResetTrigger("Backhand");
        animator.ResetTrigger("Overhand");

        if (localBallPos.y > 1.2f)
            animator.SetTrigger("Overhand");
        else if (localBallPos.x > 0f)
            animator.SetTrigger("Forehand");
        else
            animator.SetTrigger("Backhand");

    }

    void keepTrackAnimation()
    {
        if(!hasHit) return;

        timer += Time.deltaTime;

        if(timer >= 1f)
        {
            TriggerHit();
        }
    }
    void TriggerHit()
    {
        ballLaunchScript.isItPlayerSwinging = false;
        ballLaunchScript.state = BallLaunch.BallState.Hit;
    }
}
