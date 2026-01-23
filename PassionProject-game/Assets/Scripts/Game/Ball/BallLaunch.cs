using System.IO.Compression;
using UnityEngine;

public class BallLaunch : MonoBehaviour
{
    //to fix:
    /*
    - collision with glass or cage
    - going outside cage
    - player launch differs based on mpu data
    - a bit of variation in npc hit as well
    - points
    */

    public EspUdp espData;
    public PlayerHit playerHit;
    public enum BallState
    {
        Idle,
        Serving,
        Hit,
        Flying,
        Floating
    }

    public BallState state = BallState.Idle;
    public PointSystem pointSystemScript;

    [Header("Players")]
    public Transform[] leftSidePlayers;
    public Transform[] rightSidePlayers;

    public Collider leftSideCourt;
    public Collider rightSideCourt;

    [Header("Offsets")]
    public float randomYOffset;

    [Header("Flight")]
    public float flightTime = 0.9f;
    public float arcHeight = 2.4f;

    [Header("Bounce")]
    public float bounceHeight = 0.5f;
    public float bounceTime = 0.22f;

    [Header("Hit Assist")]
    public float hitFloatTime = 0.25f;
    public float hitFloatAmplitude = 0.03f;
    private bool withBounce;

    private Vector3 startPos;
    private Vector3 bouncePos;
    public Vector3 hitPos;

    private float timer;
    public Transform targetPlayer;
    public bool ballOnLeftSide = false;

    public BallServe ballServeScript;
    private enum FlightPhase
    {
        None,
        ToBounce,
        ToRacket,
        Float
    }

    private FlightPhase flightPhase = FlightPhase.None;

    [Header("Player swing")]
    public bool isItPlayerSwinging;

    public struct ShotProfile
    {
        public bool forceBounce;
        public bool goUp;
        public float flightTimeMultiplier;
        public float arcMultiplier;
        public float zOffset;
        public float xOffset;
        public float bounceZOffset;
        public float bounceXOffset;
        public float smashFactor; // 0 = normal, 1 = smash
    }

    private ShotProfile currentShot;

    float minX, minZ, maxX, maxZ;


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

        if (isItPlayerSwinging)
        {
            currentShot = BuildPlayerShot();
        }
        else currentShot = BuildNPCShot();

        timer += Time.deltaTime;
        ballServeScript.stateServe = BallServe.BallStateServe.Idle;

        ChooseTargetPlayer();
        withBounce = currentShot.forceBounce;

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
        getMinMax(); // get it once

        Debug.Log($"arc: {currentShot.arcMultiplier} flight:{currentShot.flightTimeMultiplier} xtrX: {currentShot.xOffset} bounce:{currentShot.forceBounce}");

        float baseX = Random.Range(-1.7f, 1.7f);
        float baseBounceZ = Random.Range(2f, 3f);
        float baseHitZ = Random.Range(-0.8f, 1.2f);

        baseX += currentShot.xOffset;
        baseBounceZ += currentShot.bounceZOffset;
        baseHitZ += currentShot.zOffset;

        //left or right from them
        Vector3 lateralOffset = targetPlayer.right * baseX;

        // floor bounce point
        bouncePos =
            targetPlayer.position +
            targetPlayer.forward * baseBounceZ +
            lateralOffset;

        bouncePos.y = -0.175f;

        bouncePos = ClampToBounds(bouncePos); //clamp -> not outside zone!!

        if (RandomValue(0.35f) && !currentShot.goUp) //bigger chance under,
        { //y offset
            randomYOffset = Random.Range(-0.15f, 0.15f); //backhand or forehand
        }
        else randomYOffset = Random.Range(0.55f, 1.05f); //overhand

        Debug.Log("Y: " + randomYOffset);
        hitPos =
            targetPlayer.position +
            targetPlayer.forward * baseHitZ +
            lateralOffset + Vector3.up * randomYOffset;

        hitPos = ClampToBounds(hitPos);
    }

    private void getMinMax()
    {
        minX = targetPlayer.GetComponent<Clamping>().minX;
        maxX = targetPlayer.GetComponent<Clamping>().maxX;
        minZ = targetPlayer.GetComponent<Clamping>().minZ;
        maxZ = targetPlayer.GetComponent<Clamping>().maxZ;
    }

    Vector3 ClampToBounds(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        return pos;
    }

    void UpdateFlight()
    {
        switch (flightPhase)
        {
            case FlightPhase.ToBounce:
                FlyToBounce();
                break;

            case FlightPhase.ToRacket:
                FlyToRacket();
                break;
        }
    }

    void FlyToBounce()
    {
        timer += Time.deltaTime;
        float duration = flightTime * currentShot.flightTimeMultiplier;
        float t = Mathf.Clamp01(timer / duration);
        //the smaller duration, the faster timer is at 1

        Vector3 pos = Vector3.Lerp(startPos, bouncePos, t);
        pos.y += Mathf.Sin(t * Mathf.PI) * arcHeight * currentShot.arcMultiplier;
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
        float duration = (withBounce ? flightTime * 0.55f : flightTime) * currentShot.flightTimeMultiplier;
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

        if (targetPlayer.name == "Player")
        {
            hitFloatTime = 0.5f;
        }
        else
        {
            hitFloatTime = 0.27f;
        }

        bool waitingForServe = false;
        if (!waitingForServe && timer >= hitFloatTime)
        {
            state = BallState.Idle; // missed hit (later: point logic)
            flightPhase = FlightPhase.None;
            pointSystemScript.AddPoint();
            waitingForServe = true;
        }
        if (waitingForServe && timer >= 1f)
        {
            ballServeScript.StartServe();
            timer = 0f;
            waitingForServe = false;
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
    }

    ShotProfile BuildPlayerShot()
    {
        ShotProfile shot = new ShotProfile();
        shot.smashFactor = 0f; //can be used to reduce float time

        float energy = playerHit.swingEnergy;

        switch (playerHit.swingType)
        {
            case PlayerHit.SwingType.Forehand:
                shot.arcMultiplier = Random.Range(1.1f, 1.45f); //higher arc
                shot.forceBounce = Random.value > 0.4f;
                shot.xOffset = playerHit.peakAx * 0.02f;//peak is often 10 -> + 0.2f
                break;

            case PlayerHit.SwingType.Backhand:
                shot.arcMultiplier = Random.Range(0.6f, 1f);
                shot.forceBounce = Random.value > 0.4f;
                shot.xOffset = playerHit.peakAx * 0.02f;
                break;

            case PlayerHit.SwingType.Overhand:
                shot.forceBounce = true;
                shot.arcMultiplier = 1f;
                if (energy > 11f)
                {
                    // Smash
                    shot.goUp = true; //bounce back higher
                    shot.smashFactor = Mathf.InverseLerp(11f, 16f, energy);
                    shot.arcMultiplier = 0.6f;
                }
                break;
        }

        if (energy > 11.5)
        { //if hit hard -> more to behind
          //if hit soft -> more towards front
            shot.bounceZOffset = Random.Range(-1f, 0f);
            shot.zOffset = Random.Range(-0.6f, 0.4f);
            shot.flightTimeMultiplier = Random.Range(0.45f, 0.8f); //faster
        }
        else
        {
            shot.bounceZOffset = Random.Range(0f, 0.6f);
            shot.zOffset = Random.Range(-0.2f, 1.6f);
            shot.flightTimeMultiplier = Random.Range(0.80f, 1.1f);
        }

        return shot;
    }

    ShotProfile BuildNPCShot()
    {
        ShotProfile shot = new ShotProfile();

        // Base values
        shot.flightTimeMultiplier = Random.Range(0.7f, 1.1f);
        shot.arcMultiplier = Random.Range(0.8f, 1.4f);
        shot.zOffset = 0f;
        shot.xOffset = 0f;
        shot.bounceXOffset = 0f;
        shot.bounceZOffset = 0f;
        shot.smashFactor = 0f;
        shot.forceBounce = Random.value > 0.4f; //in padel, players often play with a bounce
                                                //either with or without bounce
                                                // withBounce = false; //debug //if overhand do bounce
        shot.goUp = false;
        return shot;
    }
}
