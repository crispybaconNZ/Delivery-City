using UnityEngine;
using UnityEngine.InputSystem;

public class MinimapManager : MonoBehaviour {
    public static MinimapManager Instance;
    private bool _isOpen = true;
    [SerializeField] private float _timeToOpen; // seconds to open/close the minimap

    void Awake() {
        if (Instance == null) { Instance = this; }
        _isOpen = true;
    }

    public void Open() {
        transform.LeanMoveLocalX(320f, _timeToOpen);
        _isOpen = true;
    }

    public void Close() {
        transform.LeanMoveLocalX(475f, _timeToOpen);
        _isOpen = false;
    }

    public void ToggleMinimap(InputAction.CallbackContext context) {
        if (_isOpen) { 
            Close(); 
        } else { 
            Open(); 
        }
    }
}
