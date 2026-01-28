using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Setup multiverse")]
    public int pointsTillChaos;
    public bool transitionMultiverse = false;

    [Header("Sounds")]
    [SerializeField] private AudioClip CityAmbience;

    void Start()
    {
        // pointsTillChaos = Random.Range(3, 5);
        pointsTillChaos = 1;
        SoundFXManager.instance.PlayLoop(CityAmbience, transform, 0.65f);
    }

    void Update()
    {
        if (transitionMultiverse)
        {
           SoundFXManager.instance.StopLoop();
        }
    }

}
