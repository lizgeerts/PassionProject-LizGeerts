using System.IO.Compression;
using UnityEngine;

public class BallLaunch : MonoBehaviour
{
    //to fix:
    /*
    - animation rotation
    - collision with glass or cage
    - going outside cage
    - player launch differs based on mpu data
    - a bit of variation in npc hit as well
    - 

    */

    public enum BallState
    {
        Idle,
        Serving,
        Hit,
        Flying,
        Floating
    }

    public BallState state = BallState.Idle;

    [Header("Players")]
    public Transform[] leftSidePlayers;
    public Transform[] rightSidePlayers;

    public Collider leftSideCourt;
    public Collider rightSideCourt;

    [Header("Offsets")]
    public float xoffset = 0.0f;
    public Vector3 racketOffset = new Vector3(0f, 1.0f, 0.3f);

    [Header("Flight")]
    public float flightTime = 0.9f;
    public float arcHeight = 2.4f;
    public float sideOffsetRange = 1.2f;

    [Header("Bounce")]
    public float bounceHeight = 0.5f;
    public float bounceTime = 0.22f;

    [Header("Hit Assist")]
    public float slowDownDistance = 1.2f;
    public float hitFloatTime = 0.25f;
    public float hitFloatAmplitude = 0.03f;

    public bool isItPlayerSwinging;
    private bool withBounce;

    private Vector3 startPos;
    private Vector3 bouncePos;
    public Vector3 hitPos;

    private float timer;
    public Transform targetPlayer;
    private bool ballOnLeftSide = false;

    public BallServe ballServeScript;
    public float randomZOffset;

    private enum FlightPhase
    {
        None,
        ToBounce,
        Bounce,
        ToRacket,
        Float
    }

    private FlightPhase flightPhase = FlightPhase.None;


    void Start()
    {

    }

    void Update()
    {

        switch (state)
        {
            case BallState.Serving:
                // Serve code in ballserve script
                break;

            case BallState.Hit:
                BeginLaunch();
                break;

            case BallState.Flying:
                UpdateFlight();
                break;

            case BallState.Floating:
                UpdateFloat();
                break;
        }


    }

    void BeginLaunch()
    {
        timer += Time.deltaTime;
        ballServeScript.stateServe = BallServe.BallStateServe.Idle;

        ChooseTargetPlayer();
        withBounce = Random.value > 0.4f; //in padel, players often play with a bounce
                                          //either with or without bounce
                                          // withBounce = false; //debug

        startPos = transform.position;
        //timer = 0f;

        if (timer >= 0.25f)
        {
            CalculateTargetPositions();
            timer = 0f;
            flightPhase = withBounce ? FlightPhase.ToBounce : FlightPhase.ToRacket;
            state = BallState.Flying;
            Debug.Log(flightPhase);
        }
    }

    private bool RandomValue(float chance)
    {
        if (Random.value > chance)
        {
            return true;
        }
        else return false;
    }

    void CalculateTargetPositions()
    {
        float xOffset = Random.Range(-1.7f, 1.7f);
        Vector3 lateralOffset = targetPlayer.right * xOffset;

        // floor bounce point
        float bounceForwardDistance = 2.5f;
        bouncePos =
            targetPlayer.position +
            targetPlayer.forward * bounceForwardDistance +
            lateralOffset;

        bouncePos.y = -0.173f;

        float racketForwardDistance = Random.Range(-0.7f , 1.1f);
        if (RandomValue(0.4f)) //bigger chance under
        {
            randomZOffset = Random.Range(-0.15f, 0.15f); //backhand or forehand
        }
        else randomZOffset = Random.Range(0.55f, 1.05f); //overhand

        Debug.Log("Z: " + randomZOffset);
        hitPos =
            targetPlayer.position +
            targetPlayer.forward * racketForwardDistance +
            lateralOffset + Vector3.up * randomZOffset;

    }


    void UpdateFlight()
    {
        switch (flightPhase)
        {
            case FlightPhase.ToBounce:
                FlyToBounce();
                break;

            case FlightPhase.Bounce:
                BounceUp();
                break;

            case FlightPhase.ToRacket:
                FlyToRacket();
                break;
        }
    }

    void FlyToBounce()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / flightTime);

        Vector3 pos = Vector3.Lerp(startPos, bouncePos, t);
        pos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;
        transform.position = pos;

        if (t >= 1f)
        {
            timer = 0f;
            startPos = transform.position;
            flightPhase = FlightPhase.ToRacket;
        }
    }

    void BounceUp()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / bounceTime);

        float bounceCurve = Mathf.Sin(t * Mathf.PI * 0.85f);

        Vector3 pos = bouncePos;
        pos.y += Mathf.Sin(t * Mathf.PI * 0.85f) * bounceHeight;
        transform.position = pos;

        if (t >= 1f)
        {
            timer = 0f;
            startPos = transform.position;
            flightPhase = FlightPhase.ToRacket;
        }
    }

    void FlyToRacket()
    {
        timer += Time.deltaTime;
        float duration = withBounce ? flightTime * 0.55f : flightTime;
        float t = Mathf.Clamp01(timer / duration);

        float easedT = 1f - Mathf.Pow(1f - t, 2.4f);

        Vector3 pos = Vector3.Lerp(startPos, hitPos, easedT);
        if (!withBounce)
        {
            pos.y += Mathf.Sin(t * Mathf.PI) * 1.2f; //smaller arc
            //only when straight to the racket, apply arc height
        }

        transform.position = pos;

        if (t >= 1f)
        {
            timer = 0f;
            flightPhase = FlightPhase.Float;
            state = BallState.Floating;
        }
    }

    //Float = time to hit!

    void UpdateFloat()
    {
        timer += Time.deltaTime;

        Vector3 pos = hitPos;
        pos.y += Mathf.Sin(Time.time * 30f) * hitFloatAmplitude;
        transform.position = pos;

        if (timer >= hitFloatTime)
        {
            state = BallState.Idle; // missed hit (later: point logic)
            flightPhase = FlightPhase.None;
        }
    }

    //helper functions

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Left"))
        {
            ballOnLeftSide = true;
        }
        else if (other.CompareTag("Right"))
        {
            ballOnLeftSide = false;
        }
    }

    void ChooseTargetPlayer()
    {
        Transform[] targets = ballOnLeftSide ? rightSidePlayers : leftSidePlayers;
        targetPlayer = targets[Random.Range(0, targets.Length)];
        Debug.Log(targetPlayer.name);
    }
}
