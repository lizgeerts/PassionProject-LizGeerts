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
        scenario1, //camera moves around
        scenario2, //super speed
        scenario3, // black hole in tbhe middle, ball towards it, teleports somewhere else, could also fly higher, particle system
        scenario4, //mirroring
        scenario5
    }

    private SpaceScenarios currentScenario = SpaceScenarios.None;

    [SerializeField] private float scenarioDuration;
    public bool scenarioEnabled = false; // is a scenario currently active?

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

    [Header("scen3: black hole")]
     public GameObject BlackHole;
    public bool Scenario3 = false;


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
    }

    public void ToggleScenario()
    {
        if (scenarioEnabled)
        {
            EndScenario();
        }
        else
        {
            ChooseScenario();
            scenarioEnabled = true;
        }
    }

    void ChooseScenario()
    {
        // currentScenario = (SpaceScenarios)Random.Range(1, 3);
        currentScenario = SpaceScenarios.scenario3;
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


    void EndScenario()
    {
        Debug.Log("Scenario ended: " + currentScenario);

        StopAllScenarios();
        currentScenario = SpaceScenarios.None;
        scenarioEnabled = false;
    }

    void ResetScenario()
    {
        if (!scenarioEnabled) return;
        StopAllScenarios();
        currentScenario = SpaceScenarios.None;
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
        StopCoroutine(HyperRallySpeedRamp());

        //clean up scene 3:
        BlackHole.SetActive(false);
        BlackHole.GetComponent<ParticleSystem>().Stop();
        Scenario3 = false;
        StopCoroutine(AnimateBlackHoleRise());
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

    // ------- 4, black hole:  -------

    void StartBlackHole()
    {
        StartCoroutine(AnimateBlackHoleRise());
        Scenario3 = true;
    }

    IEnumerator AnimateBlackHoleRise()
    {
        Vector3 startPos = new Vector3(BlackHole.transform.position.x, -2.08f, BlackHole.transform.position.z);
        Vector3 endPos = new Vector3(BlackHole.transform.position.x, 0.5f, BlackHole.transform.position.z);
        float timer = 0f;

        BlackHole.transform.position = startPos;
        BlackHole.gameObject.SetActive(true);
        BlackHole.GetComponent<ParticleSystem>().Play();

        while (timer < 3f)
        {
            timer += Time.deltaTime;
            BlackHole.transform.position = Vector3.Lerp(startPos, endPos, timer / 3f);
            yield return null;
        }

        BlackHole.transform.position = endPos;
    }



    void StartMirrorMatch()
    {

    }
}