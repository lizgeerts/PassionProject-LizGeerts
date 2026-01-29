using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Setup multiverse")]
    public int pointsTillChaos;
    public bool transitionMultiverse = false;

    [Header("Sounds")]
    [SerializeField] private AudioClip CityAmbience;

    [Header("Multiplayer")]
    public bool gameIsMultiplayer = false;
    [SerializeField] private GameObject Player2;
    [SerializeField] private GameObject NPC2;


    void Start()
    {
        gameIsMultiplayer = StaticData.multiplayerValueToKeep;
        // pointsTillChaos = Random.Range(3, 5);
        pointsTillChaos = 1;
        SoundFXManager.instance.PlayLoop(CityAmbience, transform, 0.65f);
        Debug.Log("multiplayer: " + gameIsMultiplayer);
        AddPlayer();
    }

    void Update()
    {
        if (transitionMultiverse)
        {
            SoundFXManager.instance.StopLoop();
        }
    }

    void AddPlayer()
    {
        if (gameIsMultiplayer)
        {
            NPC2.SetActive(false);
            Player2.SetActive(true);
        }
        else
        {
            NPC2.SetActive(true);
            Player2.SetActive(false);
        }
    }

}
