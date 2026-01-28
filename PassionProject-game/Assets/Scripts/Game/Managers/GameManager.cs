using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("Sounds")]
    [SerializeField] private AudioClip CityAmbience;

    void Start()
    {
        SoundFXManager.instance.PlayLoop(CityAmbience, transform, 0.65f);
    }
}
