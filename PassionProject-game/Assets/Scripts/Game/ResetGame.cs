using UnityEngine;
using UnityEngine.SceneManagement; // to reload the scene

public class ResetGame : MonoBehaviour
{
    public Ballcontroller ball;        // reference to the ball
    public int maxBouncesPerSide = 3;  // set how many bounces trigger reset

    void Update()
    {
        if (ball == null) return;

        // 1. Ball bounced too many times on one side
        if (ball.leftSide && ball.bounceCount >= maxBouncesPerSide)
        {
            ResetCourt();
        }
        else if (ball.rightSide && ball.bounceCount >= maxBouncesPerSide)
        {
            ResetCourt();
        }

        // 2. Ball has left the court entirely (neither side)
        if (!ball.leftSide && !ball.rightSide)
        {
            ResetCourt();
        }
    }

    private void ResetCourt()
    {
        Debug.Log("Resetting game!");

        // Option 1: reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Option 2: if you want to reset ball/NPC positions without reloading scene,
        // you can implement a ResetPositions() method instead

    }
}

