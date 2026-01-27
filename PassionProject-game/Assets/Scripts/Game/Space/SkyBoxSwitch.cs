using UnityEngine;

public class SkyboxSwitcher : MonoBehaviour
{
    public Material skyboxMaterial;
    public bool space = false;

    void Update()
    {
        if (space)
        {
            RenderSettings.skybox = skyboxMaterial;
            DynamicGI.UpdateEnvironment();
        }
    }
}