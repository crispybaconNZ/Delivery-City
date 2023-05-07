using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour {
    private int currentlySelected;  // 0-indexed
    private PlayerInput playerInput;
    private PlayerControls playerControls;
    private UIManager ui;

    [Header("Buttons")]
    [SerializeField] private Image[] buttons;

    [Header("Button backgrounds")]
    [SerializeField] private Sprite selected;
    [SerializeField] private Sprite unselected;

    private void Awake() {
        EnablePlayerInput();
    }

    private void EnablePlayerInput() {
        playerInput = GetComponent<PlayerInput>();
        playerControls = new PlayerControls();
        playerControls.MainMenu.Move.performed += ChangeSelection;
        playerControls.MainMenu.Select.performed += ButtonSelected;
        playerControls.MainMenu.Enable();
    }

    private void DisablePlayerInput() {
        if (playerControls == null) { return; }

        playerControls.MainMenu.Move.performed -= ChangeSelection;
        playerControls.MainMenu.Select.performed -= ButtonSelected;
        playerControls.MainMenu.Disable();
    }


    private void Start() {
        currentlySelected = 0;
        ui = UIManager.Instance;
    }

    private void UpdateButtonBackgrounds() {
        for (int index =0; index < buttons.Length; index++) {
            if (index == currentlySelected) {
                buttons[index].sprite = selected;
            } else {
                buttons[index].sprite = unselected;
            }
        }
    }

    private void ChangeSelection(InputAction.CallbackContext context) {
        float dir = context.action.ReadValue<float>();
        if (dir == 0) {  return; }

        if (dir > 0) {
            // down
            currentlySelected--;
            if (currentlySelected < 0) { currentlySelected = buttons.Length - 1; }
        } else {
            // up
            currentlySelected++;
            if (currentlySelected >= buttons.Length) { currentlySelected = 0; }
        }
        UpdateButtonBackgrounds();
    }

    private void ButtonSelected(InputAction.CallbackContext context) {
        switch (currentlySelected) {
            case 0: {
                    DisablePlayerInput();
                    ui.StartNewGame();
                    break;
                }
            case 1: {
                    DisablePlayerInput();
                    ui.ShowCredits();
                    break;
                }
            case 2: {
                    DisablePlayerInput();
                    ui.ExitGame();
                    break;
                }
            default: {
                    Debug.Log($"Unknown selection {currentlySelected}");
                    break;
                }
        }
    }
}
