using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public EspUdp espData;
    public Animator playerAnimation;
    //public EspConnect espData; => if using via cable not wifi
    public float moveSpeed = 3f;

    private bool lastRunLeft;

    void Start()
    {

    }

    void Update()
    {
        Vector3 move = Vector3.zero;

        int dir = espData.joystickDir;
        bool isMoving = false;

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

        float animationDir = 0f;
        if (isMoving)
        {
            animationDir = lastRunLeft ? 1 : -1;
        }

        playerAnimation.SetFloat("Direction", animationDir);

        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}
