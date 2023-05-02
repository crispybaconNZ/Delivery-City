using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {
    public static PlayerMovement Instance = null;
    private PlayerInput playerInput;
    [SerializeField] private float _speed = 0.5f;
    private Vector3 currentDir;
    private CityBlock currentBuilding;
    private bool noticeboardOpen = false;
    private bool pauseMenuOpen = false;
    private Job _currentJob = null;
    private int _currentCash = 0;
    public Job CurrentJob { get { return _currentJob; } }
    public int CurrentCash { get { return _currentCash; } }
    public List<CityBlock> buildings = new List<CityBlock>();
    private bool _pickedupPackage;   // false if hasn't picked up package yet, true if has package to be delivered

    [SerializeField] private Transform pickupMarker;
    [SerializeField] private Transform deliveryMarker;

    // ----- EVENTS -----
    [System.Serializable] public class PlayerMovementEvent : UnityEvent<BuildingSO> { }
    public PlayerMovementEvent onEnterBuildingZone;
    public PlayerMovementEvent onLeaveBuildingZone;

    [System.Serializable] public class UIEvent : UnityEvent { }
    public UIEvent toggleNoticeboard;
    public UIEvent togglePauseMenu;
    public UIEvent cashChanged;
    
    // ----- ANIMATIONS -----
    [SerializeField] private Animator noticeboardAnim;
    [SerializeField] private Animator pauseMenuAnim;
    [SerializeField] private Animator gameOverAnim;

    void Awake() {
        if (Instance == null) { Instance = this; }
        playerInput = GetComponent<PlayerInput>();
        PlayerControls playerControls = new PlayerControls();
        playerControls.Player.Movement.performed += PlayerMove;
        playerControls.Player.Movement.canceled += PlayerMove;
        playerControls.Player.Interact.performed += PlayerInteract;
        playerControls.Player.Noticeboard.performed += ToggleNoticeboard;
        playerControls.Player.PauseMenu.performed += TogglePauseMenu;
        playerControls.Player.Enable();
        currentDir = Vector3.zero;
        currentBuilding = null;

        if (onEnterBuildingZone == null) { onEnterBuildingZone = new PlayerMovementEvent(); }
        if (onLeaveBuildingZone == null) { onLeaveBuildingZone = new PlayerMovementEvent(); }
        if (cashChanged == null) { cashChanged = new UIEvent(); }
    }

    void Update() {
        transform.position += currentDir * _speed * Time.deltaTime;

        if (CurrentJob != null) {
            CurrentJob.ReduceTime(Time.deltaTime);

            if (CurrentJob.Expired) {
                // failed to deliver on time!
                CitySceneUIManager.Instance.ShowMessage($"Delivery overdue, you lose {CurrentJob.penalty}");
                EarnPenalty(CurrentJob.penalty);
                _currentJob = null;
            }
        }
        
        if (CurrentCash <= 0) {
            GameOver();
        }
    }
    private void Start() {
        SetCash(1);
        FindAllBuildings();
        deliveryMarker.gameObject.SetActive(false);
        pickupMarker.gameObject.SetActive(false);
    }

    private void FindAllBuildings() {
        CityBlock[] locations = GameObject.FindObjectsOfType<CityBlock>();
        buildings.Clear();
        for (int i = 0; i < locations.Length; i++) {
            if (!locations[i].CompareTag("Empty Lot")) {
                buildings.Add(locations[i]);
            }
        }
    }

    private CityBlock GetBuilding(BuildingSO building) {
        foreach (CityBlock c in buildings) {
            if (c.building == building) {
                return c;
            }
        }
        return null;
    }

    private void PlayerMove(InputAction.CallbackContext context) {
        if (CitySceneUIManager.Instance.CurrentGameMode != GameMode.Playing) {
            // Ignore movement controls if not in play mode
            currentDir = Vector3.zero;
            return;
        }

        Vector3 dir = context.action.ReadValue<Vector2>();

        if (dir.magnitude > 0f) {
            currentDir = dir;
        } else {
            currentDir = Vector3.zero;
        }
    }

    private void PlayerInteract(InputAction.CallbackContext context) {
        // only allow interact if in GameMode.Playing and the building is interactable
        if (!CanInteract() || CitySceneUIManager.Instance.CurrentGameMode != GameMode.Playing) return;

        // It's a building and there's a customer, so can interact
        // if it's the target of the current job
        if (CurrentJob != null) {
            if (!_pickedupPackage) {
                if (currentBuilding.building == CurrentJob.origin) {
                    CitySceneUIManager.Instance.ShowMessage("Here's the package!");
                    HidePickupMarker();
                    PutDeliveryMarket(GetBuilding(CurrentJob.destination));
                    _pickedupPackage = true;
                } else {
                    CitySceneUIManager.Instance.ShowMessage("Sorry, I don't have anything for you");
                }
            } else {
                // have a package for delivery
                if (currentBuilding.building == CurrentJob.destination) {
                    CitySceneUIManager.Instance.ShowMessage($"Thanks! Here's your ${CurrentJob.reward}!");
                    HideDeliveryMarker();
                    EarnMoney(CurrentJob.reward);
                    _pickedupPackage = false;
                    _currentJob = null;
                } else {
                    CitySceneUIManager.Instance.ShowMessage("Sorry, I'm not expecting any deliveries");
                }
            }
        } else {
            CitySceneUIManager.Instance.ShowMessage("Nobody's home...");
        }
    }

    private bool CanInteract() {
        return currentBuilding != null && currentBuilding.building.customer != null;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Building")) {
            currentBuilding = collision.GetComponent<CityBlock>();
            onEnterBuildingZone?.Invoke(collision.GetComponent<CityBlock>().building);
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Building")) {
            currentBuilding = null;
            onLeaveBuildingZone?.Invoke(collision.GetComponent<CityBlock>().building);
        }
    }

    private void OpenNoticeboard() {
        if (CitySceneUIManager.Instance.CurrentGameMode != GameMode.Playing) return;
        noticeboardAnim.SetTrigger("OpenNoticeboard");
        noticeboardOpen = true;
        AudioManager.Instance.PlaySwoosh();
        CitySceneUIManager.Instance.CurrentGameMode = GameMode.Noticeboard;
    }

    private void CloseNoticeboard() {
        if (CitySceneUIManager.Instance.CurrentGameMode != GameMode.Noticeboard) return;
        noticeboardAnim.SetTrigger("CloseNoticeboard");
        noticeboardOpen = false;
        AudioManager.Instance.PlaySwoosh();
        CitySceneUIManager.Instance.CurrentGameMode = GameMode.Playing;
    }

    private void ToggleNoticeboard(InputAction.CallbackContext context) {
        if (noticeboardOpen) {
            CloseNoticeboard();
        } else {
            OpenNoticeboard();
        }
    }

    public void ClaimJob(Job job) {
        CloseNoticeboard();
        _currentJob = job;
        _pickedupPackage = false;

        PutPickupMarker(GetBuilding(job.origin));
    }

    private void PutPickupMarker(CityBlock origin) {
        if (origin == null) return;
        pickupMarker.transform.position = origin.transform.position;
        pickupMarker.gameObject.SetActive(true);
    }

    private void HidePickupMarker() {
        pickupMarker.gameObject.SetActive(false);
    }

    private void HideDeliveryMarker() {
        deliveryMarker.gameObject.SetActive(false);
    }

    private void PutDeliveryMarket(CityBlock destination) {
        if (destination == null) return;
        deliveryMarker.transform.position = destination.transform.position;
        deliveryMarker.gameObject.SetActive(true);
    }

    private void OpenPauseMenu() {
        if (CitySceneUIManager.Instance.CurrentGameMode != GameMode.Playing) return;
        pauseMenuAnim.SetTrigger("PauseMenuOpen");
        pauseMenuOpen = true;
        AudioManager.Instance.PlaySwoosh();
        CitySceneUIManager.Instance.CurrentGameMode = GameMode.PauseMenu;
    }

    private void ClosePauseMenu() {
        if (CitySceneUIManager.Instance.CurrentGameMode != GameMode.PauseMenu) return;
        pauseMenuAnim.SetTrigger("PauseMenuClose");
        pauseMenuOpen = false;
        AudioManager.Instance.PlaySwoosh();
        CitySceneUIManager.Instance.CurrentGameMode = GameMode.Playing;
    }

    private void TogglePauseMenu(InputAction.CallbackContext context) {
        if (pauseMenuOpen) {
            ClosePauseMenu();
        } else {
            OpenPauseMenu();
        }
    }

    public void ResumeGame() {
        ClosePauseMenu();
    }

    public void SetCash(int amount) {
        _currentCash = Mathf.Abs(amount);
        cashChanged?.Invoke();
    }

    public void EarnMoney(int amount) {
        _currentCash += amount;
        cashChanged?.Invoke();
        AudioManager.Instance.PlayKaching();
    }

    public void EarnPenalty(int amount) {
        _currentCash -= Mathf.Abs(amount);
        cashChanged?.Invoke();
        AudioManager.Instance.PlaySadTrombone();
    }

    public void GameOver() {
        gameOverAnim.SetTrigger("GameOver");
    }

    public void ReturnToMain() {
        SceneManager.LoadScene(0);
    }
}
