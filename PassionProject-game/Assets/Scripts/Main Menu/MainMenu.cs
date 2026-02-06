using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject playersScreen;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject instructionScreen;

    [SerializeField] private TextMeshProUGUI winText;
    private bool hasShownWinningScreen = false;


    void Start()
    {
        startScreen.SetActive(true);
        playersScreen.SetActive(false);
        endScreen.SetActive(false);
        instructionScreen.SetActive(false);
    }

    void Update()
    {
        if (!StaticData.showWinningScreen) return;

        if (!hasShownWinningScreen)
        {
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
        
    }

    private void ShowWinningScreen()
    {
        if(StaticData.winningTeam == 0)
        {
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
        } else
        {
            if (StaticData.multiplayerValueToKeep)
            {
                winText.SetText("Player 2 won!");
                winText.fontSize = 10f;
            }
            else
            {
                winText.SetText("You lost!");
                winText.fontSize = 13f;
            }
        }

        startScreen.SetActive(false);
        playersScreen.SetActive(false);
        endScreen.SetActive(true);   
        hasShownWinningScreen = true;     
    }
}
