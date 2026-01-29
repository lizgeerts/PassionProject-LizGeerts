using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject playersScreen;


    void Start()
    {
        startScreen.SetActive(true);
        playersScreen.SetActive(false);
    }

    public void Play()
    {
        startScreen.SetActive(false);
        playersScreen.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void setMultiplayer()
    {
        bool multiplayer = true;
        StaticData.multiplayerValueToKeep = multiplayer;
        SceneManager.LoadScene("Padel");
    }

    public void setSingleplayer()
    {
        bool multiplayer = false;
        StaticData.multiplayerValueToKeep = multiplayer;
        SceneManager.LoadScene("Padel");
    }
}
