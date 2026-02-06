using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public static PlayerInput playerInput;

    private InputAction _menuOpenAction;
    private InputAction _menuCloseAction;

    public bool MenuOpenInput { get; private set; }
    public bool MenuUIInput { get; private set; }

    // Update is called once per frame
    void Update()
    {
        MenuOpenInput = _menuOpenAction.WasPressedThisFrame();
        if (MenuOpenInput)
        {
            Pausemanager.instance.PauseGame();
        }

        MenuUIInput = _menuCloseAction.WasPressedThisFrame();
        if (MenuUIInput)
        {
            Pausemanager.instance.UnPauseGame();
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        playerInput = GetComponent<PlayerInput>();
        _menuOpenAction = playerInput.actions["MenuOPEN"];
        _menuCloseAction = playerInput.actions["MenuCLOSE"];
    }
}
