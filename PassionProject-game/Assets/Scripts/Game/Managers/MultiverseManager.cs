using UnityEngine;
using Unity.Cinemachine;

public class MultiverseManager : MonoBehaviour
{
  public GameManager gameManager;
  public CinemachineCamera playerCamGO;
  public CinemachineCamera lookUpCamGO;
  public CinemachineCamera playerCamGO2;
  public CinemachineCamera lookUpCamGO2;

  public Material citySkybox;
  public Material spaceSkybox;

  public BallLaunch ballLaunchScript;

  float timer = 0;
  public float skyboxFadeDuration = 1.5f;
  private bool skyboxSwapped = false;
  private float startExposure = 1f;

  [SerializeField] private float timerTreshold;
  [SerializeField] private GameObject city;
  [SerializeField] private GameObject space;
  [SerializeField] private AudioClip SpaceRumble;
  public bool soundPlayed = false;

  [Header("space")]
  public bool inSpace = false;
  [SerializeField] private float spaceStartDelay = 1.0f; // delay after transition before weird stuff happens
  private float spaceDelayTimer = 0f;
  private bool waitingForSpaceStart = false;



  void Start()
  {
    RenderSettings.skybox.SetFloat("_Exposure", 1);
    space.SetActive(false);
    city.SetActive(true);
    inSpace = false;
  }

  void FixedUpdate() //for camera and physics
  {

    if (gameManager.transitionMultiverse)
    {
      timer += Time.deltaTime;

      if (timer >= 1.6f) //wait for player pos reset
      {
        if (!soundPlayed)
        {
          SoundFXManager.instance.PlaySoundFXClip(SpaceRumble, transform, 0.3f, 6f);
          soundPlayed = true;
        }
        SwitchToLookUp();
        TransitionSkyboxes();
      }

      if (timer >= timerTreshold)
      {
        SwitchBack();
        timer = 0;
        gameManager.transitionMultiverse = false;

        waitingForSpaceStart = true;
        spaceDelayTimer = 0f;

        soundPlayed = false;
      }
    }

    if (waitingForSpaceStart)
    {
      spaceDelayTimer += Time.deltaTime;

      if (spaceDelayTimer >= spaceStartDelay)
      {
        inSpace = true;
        waitingForSpaceStart = false;
      }
    }
  }

  void SwitchToLookUp()
  {
    lookUpCamGO.transform.position = playerCamGO.transform.position;
    lookUpCamGO.transform.position = lookUpCamGO.transform.position + new Vector3(0, 2f, 0);
    playerCamGO.Priority = 0;
    lookUpCamGO.Priority = 1;
    if (gameManager.gameIsMultiplayer)
    {
      lookUpCamGO2.transform.position = playerCamGO2.transform.position;
      lookUpCamGO2.transform.position = lookUpCamGO2.transform.position + new Vector3(0, 2f, 0);
      playerCamGO2.Priority = 0;
      lookUpCamGO2.Priority = 1;
    }
  }

  void SwitchBack()
  {
    SwitchEnvironment();
    playerCamGO.Priority = 1;
    lookUpCamGO.Priority = 0;
    if (gameManager.gameIsMultiplayer)
    {
      playerCamGO2.Priority = 1;
      lookUpCamGO2.Priority = 0;
    }
    ballLaunchScript.state = BallLaunch.BallState.Serving;
  }

  void TransitionSkyboxes()
  {
    if (!skyboxSwapped)
    {
      float t = Mathf.Clamp01(timer / skyboxFadeDuration);
      float exposure = Mathf.Lerp(startExposure, 0f, t);
      RenderSettings.skybox.SetFloat("_Exposure", exposure);

      // when faded out -> space skybox
      if (t >= 1f)
      {
        RenderSettings.skybox = spaceSkybox;
        RenderSettings.skybox.SetFloat("_Exposure", 0f);
        DynamicGI.UpdateEnvironment();

        skyboxSwapped = true;
      }
    }
    // fade in
    else
    {

      float t = Mathf.Clamp01((timer - skyboxFadeDuration) / skyboxFadeDuration);
      float exposure = Mathf.Lerp(0f, startExposure, t);
      RenderSettings.skybox.SetFloat("_Exposure", exposure);
    }
  }

  void SwitchEnvironment()
  {
    space.SetActive(true);
    city.SetActive(false);
  }
}
