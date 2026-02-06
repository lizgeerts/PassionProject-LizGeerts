using UnityEngine;
using UnityEngine.SceneManagement;

public class Pausemanager : MonoBehaviour
{

    public static Pausemanager instance;
    public bool isPaused { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject gamePauseScreen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        gamePauseScreen.SetActive(false);
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        InputManager.playerInput.SwitchCurrentActionMap("UI");
        gamePauseScreen.SetActive(true);
    }

    public void UnPauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        InputManager.playerInput.SwitchCurrentActionMap("Player");
        gamePauseScreen.SetActive(false);
    }

    public void LoadMenuScene()
    {
        Time.timeScale = 1f;
        StaticData.showWinningScreen = false;
        SceneManager.LoadScene("MainMenu");
    }
}
