using UnityEngine;
using System.Collections.Generic;

public class BallServe : MonoBehaviour
{

    public BallLaunch ballLaunchScript;
    public Transform servePlayer;
    public NpcHitBall[] npcScripts;
    public PlayerBallHit playerScript;

    [Header("Serve Settings")]
    public float serveHeight = 1.2f;
    public float serveForwardOffset = 0.4f;
    public float serveSideOffset = 0.15f;

    public float serveDropTime = 0.25f;
    public float serveReturnTime = 0.22f;
    public float serveFloatAmplitude = 0.05f;
    public float serveFloatSpeed = 3f;
    public enum BallStateServe
    {
        Idle,
        ServeDrop,
        ServeReturn,
        ServeFloat
    }
    public BallStateServe stateServe = BallStateServe.Idle;
    Vector3 serveAnchorPos;
    Vector3 groundPos;

    private float timer;

    // bounce tracking per side
    public int leftSideBounces = 0;
    public int rightSideBounces = 0;

    [Header("Sounds")]
    [SerializeField] private AudioClip ballBounceClip;

    void Start()
    {
        StartServe();
    }

    void Update()
    {
        if (ballLaunchScript.state != BallLaunch.BallState.Serving)
            return;

        switch (stateServe)
        {
            case BallStateServe.ServeDrop:
                UpdateServeDrop();
                break;

            case BallStateServe.ServeReturn:
                UpdateServeReturn();
                break;

            case BallStateServe.ServeFloat:
                UpdateServeFloat();
                break;
        }
    }

    // ----------------------------
    // SERVE SEQUENCE
    // ----------------------------

    public void StartServe()
    {
        // Reset all NPCs!
        if (npcScripts != null)
        {
            for (int i = 0; i < npcScripts.Length; i++)
            {
                if (npcScripts[i] != null)
                    npcScripts[i].ResetToStart();
            }
        }

        playerScript.ResetToStart();

        ballLaunchScript.state = BallLaunch.BallState.Serving;
        // Base serve position
        serveAnchorPos =
            servePlayer.position +
            servePlayer.forward * serveForwardOffset +
            servePlayer.right * serveSideOffset +
            Vector3.up * serveHeight;

        groundPos = new Vector3(
            serveAnchorPos.x,
            -0.175f, // court height
            serveAnchorPos.z
        );

        transform.position = serveAnchorPos;
        timer = 0f;
        stateServe = BallStateServe.ServeDrop;
    }

    void UpdateServeDrop()
    {
        timer += Time.deltaTime;
        float t = timer / serveDropTime;

        transform.position = Vector3.Lerp(serveAnchorPos, groundPos, t);

        if (t >= 1.1f)
        {
            SoundFXManager.instance.PlaySoundFXClip(ballBounceClip, transform, 0.7f, 0f);
            timer = 0f;
            stateServe = BallStateServe.ServeReturn;
        }
    }

    void UpdateServeReturn()
    {
        timer += Time.deltaTime;
        float t = timer / serveReturnTime;

        transform.position = Vector3.Lerp(groundPos, serveAnchorPos, t);

        if (t >= 1.1f)
        {
            stateServe = BallStateServe.ServeFloat;
        }
    }

    void UpdateServeFloat()
    {
        timer += Time.deltaTime;

        Vector3 pos = serveAnchorPos;
        pos.y += Mathf.Sin(Time.time * serveFloatSpeed) * serveFloatAmplitude;
        transform.position = pos;
        //floats waiting for player to hit

        if (timer >= 1.1f && ballLaunchScript.state == BallLaunch.BallState.Serving)
        {
            stateServe = BallStateServe.Idle;
            StartServe();
        } //if they missed the serve, then try again
    }

}
