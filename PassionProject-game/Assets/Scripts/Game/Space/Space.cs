using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using Unity.Mathematics;

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
        scenario4, // ball size changes
        scenario5 // gravity party
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

    [Header("scen4: ball sizes")]
    [SerializeField] private GameObject ball;

    [Header("scen5: gravity party")]
    [SerializeField] private Transform[] characters;
    private Vector3[] originalPositions;
    private Quaternion[] originalRotations;


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

        OriginalCharacterTransforms();
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
        currentScenario = SpaceScenarios.scenario5;
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
                StartBallSize();
                break;

            case SpaceScenarios.scenario5:
                GravityParty();
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
        StopAllCoroutines();

        //cleanup scene 1:
        if (chaosRoutine != null)
        {
            //StopCoroutine(chaosRoutine);
            chaosRoutine = null;
        }
        if (p1Composer != null)
            p1Composer.TargetOffset = p1OriginalOffset;

        if (p2Composer != null)
            p2Composer.TargetOffset = p2OriginalOffset;

        //clean up scene 2:
        ballLaunchScript.hyperRallyMultiplyer = 1f;
        //StopCoroutine(HyperRallySpeedRamp());

        //clean up scene 3:
        BlackHole.SetActive(false);
        BlackHole.GetComponent<ParticleSystem>().Stop();
        Scenario3 = false;
       // StopCoroutine(AnimateBlackHoleRise());

        //clean up scene 4:
        ball.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);

        //clean up scene 5:
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].position = originalPositions[i];
            Vector3 eulerRot = originalRotations[i].eulerAngles;
            eulerRot.z = 0f;  // extra reset Z rotation 
            characters[i].rotation = Quaternion.Euler(eulerRot);
        }
        ballLaunchScript.bounceHeight = -0.175f;
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

    // ------- 3, black hole:  -------

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

    // ------- 4, ball sizes:  -------

    void StartBallSize()
    {
        StartCoroutine(AnimateBallSizes());
    }

    IEnumerator AnimateBallSizes()
    {
        Vector3 originalScale = ball.transform.localScale;

        float minScale = 0.015f;
        float maxScale = 0.45f;
        float pulseSpeed = 2.5f;

        float t = 0f;

        while (true)
        {
            t += Time.deltaTime * pulseSpeed;

            float pulse = (Mathf.Sin(t) + 1f) * 0.5f; // 0 → 1
            float scaleValue = Mathf.Lerp(minScale, maxScale, pulse);

            ball.transform.localScale = originalScale.normalized * scaleValue;


            yield return null;
        }
    }

    // ------- 5, weird gravity :  -------
    void OriginalCharacterTransforms()
    {
        originalPositions = new Vector3[characters.Length];
        originalRotations = new Quaternion[characters.Length];

        for (int i = 0; i < characters.Length; i++)
        {
            originalPositions[i] = characters[i].position;
            originalRotations[i] = characters[i].rotation;
        }
    }

    void GravityParty()
    {
        StartCoroutine(GravityPartyRoutine());
    }

    IEnumerator GravityPartyRoutine()
    {
        // 1: Float up smoothly 

        float introDuration = 2f;
        float introT = 0f;

        while (introT < introDuration)
        {
            introT += Time.deltaTime;

            for (int i = 0; i < characters.Length; i++)
            {
                float introHeight = Mathf.Lerp(0f, 0.7f, introT / introDuration); // 0 → 0.7m

                Vector3 pos = characters[i].position;
                pos.y = originalPositions[i].y + introHeight;
                characters[i].position = pos;
            }

            yield return null;
        }

        // 2: Float up and down in the air
        float t = 0f;
        float floatSpeed = 1.2f;
        float floatHeight = 0.3f;
        float rotateSpeed = 1f;
        float rotateAngle= 12f;
        ballLaunchScript.bounceHeight = UnityEngine.Random.Range(0.28f, 0.77f);

        while (true)
        {
            t += Time.deltaTime;

            for (int i = 0; i < characters.Length; i++)
            {
                float phaseOffset = i * 0.6f; // makes them desync nicely

                float yOffset = Mathf.Sin(t * floatSpeed + phaseOffset) * floatHeight;
                float zRot = Mathf.Sin(t * rotateSpeed + phaseOffset) * rotateAngle;

                Vector3 pos = characters[i].position;
                pos.y = originalPositions[i].y+ yOffset + 0.7f; //+0.7 otherwise float in the

                characters[i].position = pos;
                characters[i].rotation =
                    originalRotations[i] * Quaternion.Euler(0f, 0f, zRot);
            }

            yield return null;
        }
    }

}