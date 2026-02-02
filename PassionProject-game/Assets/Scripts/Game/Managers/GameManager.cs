using TMPro;
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

    [SerializeField] private TextMeshProUGUI teamText1;
    [SerializeField] private TextMeshProUGUI teamText2;


    [Header("Cameras")]
    public Camera cameraPlayer1;
    public Camera cameraPlayer2;


    void Start()
    {
        gameIsMultiplayer = StaticData.multiplayerValueToKeep;
        setupCameras();
        // pointsTillChaos = Random.Range(3, 5);
        pointsTillChaos = 1;
        SoundFXManager.instance.PlayLoop(CityAmbience, transform, 0.65f);
        Debug.Log("multiplayer: " + gameIsMultiplayer);
        AddPlayer();
        StaticData.showWinningScreen = false;
        SetTeamText();
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

    void setupCameras()
    {
        if (gameIsMultiplayer)
        {
            // Both cameras active
            cameraPlayer1.gameObject.SetActive(true);
            cameraPlayer2.gameObject.SetActive(true);

            cameraPlayer1.rect = new Rect(0f, 0f, 0.5f, 1f);   
            cameraPlayer2.rect = new Rect(0.5f, 0f, 0.5f, 1f); 
        }
        else
        {
            // Single player: only camera 1, full screen
            cameraPlayer1.gameObject.SetActive(true);
            cameraPlayer2.gameObject.SetActive(false);

            cameraPlayer1.rect = new Rect(0f, 0f, 1f, 1f); 
        }
    }

    void SetTeamText()
    {
        if (gameIsMultiplayer)
        {
            teamText1.SetText("Team 1 (P1)");
            teamText2.SetText("Team 2 (P2)");
        }
        else
        {
            teamText1.SetText("Team 1 (you)");
            teamText2.SetText("Team 2");
        }
    }
}
