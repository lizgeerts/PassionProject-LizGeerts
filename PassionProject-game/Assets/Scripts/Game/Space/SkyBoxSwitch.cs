using UnityEngine;

public class SkyboxSwitcher : MonoBehaviour
{
    public Material skyboxMaterial;
    public bool multiverse = false;

    void Update()
    {
        if (multiverse)
        {
            RenderSettings.skybox = skyboxMaterial;
            DynamicGI.UpdateEnvironment();
        } 
    }
}