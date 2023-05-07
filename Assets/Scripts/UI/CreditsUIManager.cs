using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CreditsUIManager : MonoBehaviour {
    private PlayerInput playerInput;
    private PlayerControls playerControls;

    private void Awake() {
        playerInput = GetComponent<PlayerInput>();
        playerControls = new PlayerControls();
        playerControls.MainMenu.Select.performed += ReturnToMainMenu;
        playerControls.MainMenu.Enable();
    }
    public void ReturnToMainMenu(InputAction.CallbackContext context) {
        playerControls.MainMenu.Select.performed -= ReturnToMainMenu;
        playerControls.MainMenu.Disable();
        SceneManager.LoadScene(0);
    }
}
