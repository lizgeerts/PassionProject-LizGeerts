using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public EspUdp espData;
    public Animator playerAnimation;
    //public EspConnect espData; => if using via cable not wifi
    public float moveSpeed = 3f;

    private bool lastRunLeft;
    private bool isMoving;
    float animationDir;

    void Start()
    {

    }

    void Update()
    {
        Vector3 move = Vector3.zero;

        int dir = espData.joystickDir;
        isMoving = false;

        switch (dir)
        {
            case 1: move = transform.forward; isMoving = true; break;
            case 2: move = -transform.forward; isMoving = true; break;
            case 3:
                {
                    move = transform.right;
                    lastRunLeft = false;
                    isMoving = true;
                    break;
                }
            case 4:
                {
                    move = -transform.right;
                    lastRunLeft = true;
                    isMoving = true;
                    break;
                }
        }

        animationDir = 0f;
        if (isMoving)
        {
            animationDir = lastRunLeft ? 1 : -1;
        }
        RotatePlayer();

        playerAnimation.SetFloat("Direction", animationDir);

        controller.Move(move * moveSpeed * Time.deltaTime);
        Vector3 pos = transform.position;
        pos.y = 0.842f; //keep on the floor
        transform.position = pos;
    }

    private void RotatePlayer()
    {
        //rotate player when moving
        if (isMoving)
        {
            if (animationDir == 1)
            {
                transform.rotation = Quaternion.Euler(0, -15, 0);
            }
            else if (animationDir == -1)
            {
                transform.rotation = Quaternion.Euler(0, 15, 0);
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
