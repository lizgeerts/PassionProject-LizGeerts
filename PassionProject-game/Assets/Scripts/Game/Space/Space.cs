using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class Space : MonoBehaviour
{
    public MultiverseManager multiverseManager;
    public GameManager gameManager;

    public enum SpaceScenarios
    {
        None,
        scenario1, //camera upside down, backhand is forehand 
        scenario2, //super speed
        scenario3, // black hole in tbhe middle, ball towards it, teleports somewhere else, could also fly higher, particle system
        scenario4, //mirroring
        scenario5
    }

    private SpaceScenarios currentScenario = SpaceScenarios.None;

    private float timer = 0f;

    [SerializeField] private float scenarioDuration;
    private float scenarioTimer = 0f;
    private bool scenarioActive = false;
    [SerializeField] private float delayBeforeNextScenario = 1.0f; // 1 second delay
    private bool waitingForNextScenario = false;

    [Header("scen1: cam chaos")]
    [SerializeField] private float chaosAmplitude = 1.5f;
    [SerializeField] private float chaosSpeed = 2.0f;
    [SerializeField] private CinemachineRotationComposer p1Composer;
    [SerializeField] private CinemachineRotationComposer p2Composer;
    private Vector3 p1OriginalOffset;
    private Vector3 p2OriginalOffset;
    private Coroutine chaosRoutine;

    [Header("scen2: speed")]
    [SerializeField] private BallLaunch ballLaunchScript;
    [SerializeField] private float hyperRallySpeedMultiplier = 3f; // 3x faster
    [SerializeField] private float hyperRallyRampTime = 0.5f; // ramp up duration
    private float hyperRallyCurrentMultiplier = 1f;


    void Start()
    {
        if (p1Composer != null)
        {
            p1OriginalOffset = p1Composer.TargetOffset;
        }
        if (p2Composer != null)
        {
            p2OriginalOffset = p2Composer.TargetOffset;
        }
    }

    void Update()
    {
        if (!multiverseManager.inSpace)
        {
            ResetScenario();
            return;
        }

        if (!scenarioActive && !waitingForNextScenario)
        {
            ChooseScenario();
        }

        scenarioTimer += Time.deltaTime;

        if (scenarioTimer >= scenarioDuration)
        {
            EndScenario();
        }

        RunScenario();
    }

    void ChooseScenario()
    {
        scenarioActive = true;
        scenarioTimer = 0f;

        currentScenario = (SpaceScenarios)Random.Range(1, 3);
        Debug.Log("Scenario started: " + currentScenario);

        StartScenario(currentScenario);
    }


    void StartScenario(SpaceScenarios scenario)
    {
        switch (scenario)
        {
            case SpaceScenarios.scenario1:
                StartCamChaos();
                break;

            case SpaceScenarios.scenario2:
                StartHyperRally();
                break;

            case SpaceScenarios.scenario3:
                StartBlackHole();
                break;

            case SpaceScenarios.scenario4:
                StartMirrorMatch();
                break;
        }
    }

    void RunScenario()
    {
        switch (currentScenario)
        {
            case SpaceScenarios.scenario3:
                // UpdateBlackHole();
                break;
        }
    }

    void EndScenario()
    {
        Debug.Log("Scenario ended: " + currentScenario);

        StopAllScenarios();
        currentScenario = SpaceScenarios.None;
        scenarioActive = false;
        scenarioTimer = 0f;

        if (!waitingForNextScenario)
            StartCoroutine(ScenarioDelayCoroutine());
    }
    IEnumerator ScenarioDelayCoroutine()
    {
        waitingForNextScenario = true;
        yield return new WaitForSeconds(delayBeforeNextScenario);
        waitingForNextScenario = false;
    }
    void ResetScenario()
    {
        if (!scenarioActive) return;
        StopAllScenarios();
        currentScenario = SpaceScenarios.None;
        scenarioActive = false;
        scenarioTimer = 0f;
    }

    void StopAllScenarios()
    {
        //cleanup scene 1:
        if (chaosRoutine != null)
        {
            StopCoroutine(chaosRoutine);
            chaosRoutine = null;
        }
        if (p1Composer != null)
            p1Composer.TargetOffset = p1OriginalOffset;

        if (p2Composer != null)
            p2Composer.TargetOffset = p2OriginalOffset;

        //clean up scene 2:
        ballLaunchScript.hyperRallyMultiplyer = 1f;
    }


    // ------- Each scenario:  -------

    // ------- 1, camera movements:  -------

    void StartCamChaos()
    {
        if (chaosRoutine != null) return;
        chaosRoutine = StartCoroutine(CameraChaos());
    }

    IEnumerator CameraChaos()
    {
        float t = 0f;

        while (true)
        {
            t += Time.deltaTime * chaosSpeed;

            Vector3 offset = new Vector3(
                Mathf.Sin(t * 0.9f) * chaosAmplitude * 0.35f,
                Mathf.Sin(t * 1.3f) * chaosAmplitude * 0.5f,
                Mathf.Cos(t * 0.7f) * chaosAmplitude * 0.45f
            );

            if (p1Composer != null)
                p1Composer.TargetOffset = p1OriginalOffset + offset;

            if (gameManager.gameIsMultiplayer && p2Composer != null)
                p2Composer.TargetOffset = p2OriginalOffset + offset;

            yield return null;
        }
    }

    // ------- 2, speed:  -------

    void StartHyperRally()
    {
       //need to fix that hitting also happens faster
        StartCoroutine(HyperRallySpeedRamp());
    }


    IEnumerator HyperRallySpeedRamp()
    {
        float hypertimer = 0f;
        hyperRallyCurrentMultiplier = 1f; // start normal speed

        while (hypertimer < hyperRallyRampTime)
        {
            hypertimer += Time.deltaTime;
            hyperRallyCurrentMultiplier = Mathf.Lerp(1f, hyperRallySpeedMultiplier, hypertimer / hyperRallyRampTime);
            yield return null;
        }

        hyperRallyCurrentMultiplier = hyperRallySpeedMultiplier;
        ballLaunchScript.hyperRallyMultiplyer = hyperRallyCurrentMultiplier;
    }

    void StartBlackHole()
    {

    }

    void StartMirrorMatch()
    {

    }
}