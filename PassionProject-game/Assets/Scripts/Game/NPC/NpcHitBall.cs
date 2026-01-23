using UnityEngine;

public class NpcHitBall : MonoBehaviour
{
    [Header("References")]
    public BallLaunch ballLaunchScript;
    public Animator animator;
    public Collider npcCollider;

    public enum CourtSide { Left, Right }
    public CourtSide mySide;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float stopDistance = 0.2f;

    [Header("Hit Settings")]

    private Vector3 targetHitPos;

    private bool isBallComingToMe = false;
    private bool hasHit = false;
    private float timer = 0f;
    public float moveTimer;
    private float xOffset = 0f;
    private float zOffset = 0f;

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
        Vector3 ballPoint = ballLaunchScript.hitPos;

        // Add small random offset so NPC is not perfectly on top
        if (ballLaunchScript.randomYOffset < 0.30f) //if not overhand
        { 
            xOffset = Random.value < 0.5f ? -0.5f : 0.5f;
        }
        else if (mySide == CourtSide.Left)
        {//if overhand, only x offset to side ball is
            xOffset = 0.3f;
        }
        else xOffset = -0.3f;

        if(mySide == CourtSide.Left)
        {
            zOffset = 0.3f;
        }
        else zOffset = -0.3f;


        targetHitPos = new Vector3(
            ballPoint.x + xOffset,
            0.852f, // stay on ground 
            ballPoint.z + zOffset
        );
    }



    void MoveToTarget()
    {
        if (hasHit)
        {
            animator.SetFloat("Direction", 0f);
            return;
        }

        // move smoothly toward target
        Vector3 moveDir = targetHitPos - transform.position;
        moveDir.y = 0; //no movemeng up 
        float distance = moveDir.magnitude;

        if (distance > stopDistance)
        {
            transform.position += moveDir.normalized * moveSpeed * Time.deltaTime;

            float xDiff = targetHitPos.x - transform.position.x;
            float animDirection = 0f;

            if (Mathf.Abs(xDiff) > 0.1f)
            {
                // For CourtSide.Right (facing Forward), positive xDiff is "Right"
                // For CourtSide.Left (facing Back), positive xDiff is "Left"
                animDirection = (xDiff > 0) ? 1f : -1f;

                // Flip logic if they are on the Left side facing Backwards
                if (mySide == CourtSide.Left)
                {
                    animDirection *= -1f;
                }
            }
            animator.SetFloat("Direction", animDirection);
        }
        else
        {
            // We reached the target
            animator.SetFloat("Direction", 0f);
        }

        //lock rotation and y pos, so they dont go turning and floating
        Vector3 currentPos = transform.position;
        currentPos.y = 0.852f;
        transform.position = currentPos;

        if (mySide == CourtSide.Left)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.back);
        }
        else { transform.rotation = Quaternion.LookRotation(Vector3.forward); }
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
        animator.ResetTrigger("Forehand");
        animator.ResetTrigger("Backhand");
        animator.ResetTrigger("Overhand");

        if (ballLaunchScript.randomYOffset >= 0.55f)
            animator.SetTrigger("Overhand");
        else if (xOffset > 0f && mySide == CourtSide.Left || xOffset < 0f && mySide == CourtSide.Right)
            animator.SetTrigger("Forehand");
        else
            animator.SetTrigger("Backhand");

    }

    void keepTrackAnimation()
    {
        if (!hasHit) return;

        timer += Time.deltaTime;

        if (timer >= 0.9f)
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
