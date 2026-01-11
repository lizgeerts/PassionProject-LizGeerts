using UnityEngine;
using UnityEngine.SceneManagement; // to reload the scene

public class ResetGame : MonoBehaviour
{
    public Ballcontroller ballScript;        // reference to the ball
    public Transform ball;
    public int maxBouncesPerSide = 3;  // set how many bounces trigger reset
    public Transform NPC;
    public NpcMovement NPCscript;

    void Update()
    {
        if (ballScript == null) return;

        // 1. Ball bounced too many times on one side
        if (ballScript.leftSide && ballScript.bounceCount >= maxBouncesPerSide)
        {
            ResetCourt();
        }
        else if (ballScript.rightSide && ballScript.bounceCount >= maxBouncesPerSide)
        {
            ResetCourt();
        }

        // 2. Ball has left the court entirely (neither side)
        if (!ballScript.leftSide && !ballScript.rightSide)
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

