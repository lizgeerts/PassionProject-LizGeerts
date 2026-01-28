using UnityEngine;
using Unity.Cinemachine;

public class MultiverseManager : MonoBehaviour
{
  public GameManager gameManager;
  public CinemachineCamera playerCamGO;
  public CinemachineCamera lookUpCamGO;

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


  void Start()
  {
    RenderSettings.skybox.SetFloat("_Exposure", 1);
    space.SetActive(false);
    city.SetActive(true);
  }

  void FixedUpdate() //for camera and physics
  {
    if (gameManager.transitionMultiverse)
    {
      timer += Time.deltaTime;

      if(timer >= 1.6f) //wait for player pos reset
      {
        SoundFXManager.instance.PlaySoundFXClip(SpaceRumble, transform, 0.1f, 1.5f);
        SwitchToLookUp();
        TransitionSkyboxes();
      }

      if (timer >= timerTreshold)
      {
        SwitchBack();
        timer = 0;
        gameManager.transitionMultiverse = false;
      }
    }
  }

  void SwitchToLookUp()
  {
    lookUpCamGO.transform.position = playerCamGO.transform.position;
    lookUpCamGO.transform.position = lookUpCamGO.transform.position + new Vector3(0, 2f, 0);
    playerCamGO.Priority = 0;
    lookUpCamGO.Priority = 1;
  }

  void SwitchBack()
  {
    SwitchEnvironment();
    playerCamGO.Priority = 1;
    lookUpCamGO.Priority = 0;
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
