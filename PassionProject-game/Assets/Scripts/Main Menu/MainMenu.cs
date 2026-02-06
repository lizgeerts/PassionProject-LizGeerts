using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject playersScreen;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject instructionScreen;
    [SerializeField] private GameObject practiseBackButton;

    [SerializeField] private GameObject player;

    [SerializeField] private TextMeshProUGUI winText;
    private bool hasShownWinningScreen = false;

    [Header("Sounds")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip winningMusic;
    [SerializeField] private AudioClip lostMusic;


    void Start()
    {
        startScreen.SetActive(true);
        playersScreen.SetActive(false);
        endScreen.SetActive(false);
        instructionScreen.SetActive(false);
        player.SetActive(false);
        practiseBackButton.SetActive(false);
        SoundMainMenu.instance.PlayLoop(menuMusic, transform, true, 0.04f);
    }

    void Update()
    {
        if (!StaticData.showWinningScreen) return;

        if (!hasShownWinningScreen)
        {
            SoundMainMenu.instance.StopLoop();
            ShowWinningScreen();
        }
    }

    public void Play()
    {
        startScreen.SetActive(false);
        endScreen.SetActive(false);
        playersScreen.SetActive(true);
        instructionScreen.SetActive(false);
        hasShownWinningScreen = false;
        StaticData.showWinningScreen = false;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void setMultiplayer()
    {
        bool multiplayer = true;
        StaticData.multiplayerValueToKeep = multiplayer;
        instructionScreen.SetActive(true);
        playersScreen.SetActive(false);
        //SceneManager.LoadScene("Padel");
    }

    public void setSingleplayer()
    {
        bool multiplayer = false;
        StaticData.multiplayerValueToKeep = multiplayer;
        instructionScreen.SetActive(true);
        playersScreen.SetActive(false);
        //SceneManager.LoadScene("Padel");
    }

    public void BackToMain()
    {
        startScreen.SetActive(true);
        playersScreen.SetActive(false);
        endScreen.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Padel");
    }

    public void Practise()
    {
        player.SetActive(true);
        instructionScreen.SetActive(false);
        practiseBackButton.SetActive(true);
    }

    public void BackToInstructions()
    {
        player.SetActive(false);
        instructionScreen.SetActive(true);
        practiseBackButton.SetActive(false);
    }

    private void ShowWinningScreen()
    {
        if (StaticData.winningTeam == 0)
        {
            SoundMainMenu.instance.PlayLoop(winningMusic, transform, false, 0.19f);

            if (StaticData.multiplayerValueToKeep)
            {
                winText.SetText("Player 1 won!");
                winText.fontSize = 10f;
            }
            else
            {
                winText.SetText("You won!");
                winText.fontSize = 13f;
            }
        }
        else
        {
            if (StaticData.multiplayerValueToKeep)
            {
                SoundMainMenu.instance.PlayLoop(winningMusic, transform, false, 0.19f);
                winText.SetText("Player 2 won!");
                winText.fontSize = 10f;
            }
            else
            {
                winText.SetText("You lost!");
                winText.fontSize = 13f;
                SoundMainMenu.instance.PlayLoop(lostMusic, transform, false, 0.2f);
            }
        }

        startScreen.SetActive(false);
        playersScreen.SetActive(false);
        endScreen.SetActive(true);
        hasShownWinningScreen = true;
    }
}
