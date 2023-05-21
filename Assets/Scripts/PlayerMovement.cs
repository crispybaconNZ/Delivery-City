using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {
    public static PlayerMovement Instance;
    private PlayerInput playerInput;
    private PlayerControls playerControls;
    [SerializeField] private float _speed = 0.5f;
    private Vector3 currentDir;
    private CityBlock currentBuilding;
    private bool pauseMenuOpen = false;
    private Job _currentJob = null;
    private int _currentCash = 0;
    public Job CurrentJob { get { return _currentJob; } }
    [SerializeField] private Transform _spinner;    // arrow to point towards next destination
    public int CurrentCash { get { return _currentCash; } }
    public List<CityBlock> buildings = new List<CityBlock>();
    private bool _pickedupPackage;   // false if hasn't picked up package yet, true if has package to be delivered

    [Header("Job Location Markers")]
    [SerializeField] private Transform pickupMarker;
    [SerializeField] private Transform deliveryMarker;

    [Header("Claim Job Buttons")]
    [SerializeField] private Image claimJob1;
    [SerializeField] private Image claimJob2;
    private int _currentClaimJob = 0;

    [Header("Dialog Box Backgrounds")]
    [SerializeField] private Sprite selected;
    [SerializeField] private Sprite unselected;

    [Header("Pause Menu Buttons")]
    [SerializeField] private Image resumeGame;
    [SerializeField] private Image returnToMain;
    [SerializeField] private Image exitGame;
    private int _currentPauseMenu = 0;

    // ----- EVENTS -----
    [System.Serializable] public class PlayerMovementEvent : UnityEvent<BuildingSO> { }
    public PlayerMovementEvent onEnterBuildingZone;
    public PlayerMovementEvent onLeaveBuildingZone;

    [System.Serializable] public class UIEvent : UnityEvent { }
    public UIEvent toggleNoticeboard;
    public UIEvent togglePauseMenu;
    public UIEvent cashChanged;
    
    // ----- ANIMATIONS -----
    [SerializeField] private Animator pauseMenuAnim;
    [SerializeField] private Animator gameOverAnim;

    void Awake() {
        if (Instance == null) { Instance = this; }
        playerInput = GetComponent<PlayerInput>();
        playerControls = new PlayerControls();
        currentDir = Vector3.zero;
        currentBuilding = null;

        if (onEnterBuildingZone == null) { onEnterBuildingZone = new PlayerMovementEvent(); }
        if (onLeaveBuildingZone == null) { onLeaveBuildingZone = new PlayerMovementEvent(); }
        if (cashChanged == null) { cashChanged = new UIEvent(); }

        LeanTween.init(800);
    }

    
    private void Start() {
        SetCash(1);
        FindAllBuildings();
        deliveryMarker.gameObject.SetActive(false);
        pickupMarker.gameObject.SetActive(false);
        EnablePlayerMovement();
        DisableNoticeboardControls();
        DisablePauseMenuControls();
    }

    #region Player Controls
    void EnablePlayerMovement() {
        if (playerControls == null) { return; }
        playerControls.Player.Movement.performed += PlayerMove;
        playerControls.Player.Movement.canceled += PlayerMove;
        playerControls.Player.Interact.performed += PlayerInteract;
        playerControls.Player.Noticeboard.performed += ToggleNoticeboard;
        playerControls.Player.PauseMenu.performed += TogglePauseMenu;
        playerControls.Player.Minimap.performed += MinimapManager.Instance.ToggleMinimap;
        playerControls.Player.Enable();
    }

    void DisablePlayerMovement() {
        if (playerControls == null) { return; }
        playerControls.Player.Movement.performed -= PlayerMove;
        playerControls.Player.Movement.canceled -= PlayerMove;
        playerControls.Player.Interact.performed -= PlayerInteract;
        playerControls.Player.Noticeboard.performed -= ToggleNoticeboard;
        playerControls.Player.PauseMenu.performed -= TogglePauseMenu;
        playerControls.Player.Minimap.performed -= MinimapManager.Instance.ToggleMinimap;
        playerControls.Player.Enable();
    }

    void EnableNoticeboardControls() {
        if (playerControls == null) { return; }
        playerControls.Noticeboard.Navigate.performed += ChangeNoticeboardSelection;
        playerControls.Noticeboard.Cancel.performed += CancelNoticeboard;
        playerControls.Noticeboard.Select.performed += ClaimJob;
        playerControls.Noticeboard.Enable();
    }

    void DisableNoticeboardControls() {
        if (playerControls == null) { return; }
        playerControls.Noticeboard.Navigate.performed -= ChangeNoticeboardSelection;
        playerControls.Noticeboard.Cancel.performed -= CancelNoticeboard;
        playerControls.Noticeboard.Select.performed -= ClaimJob;
        playerControls.Noticeboard.Disable();
    }

    void EnablePauseMenuControls() {
        if (playerControls == null) { return; }
        playerControls.PauseMenu.Navigate.performed += ChangePauseMenuSelection;
        playerControls.PauseMenu.Select.performed += PauseMenuSelect;
        playerControls.PauseMenu.Cancel.performed += CancelPauseMenu;
        playerControls.PauseMenu.Enable();
    }

    void DisablePauseMenuControls() {
        if (playerControls == null) { return; }
        playerControls.PauseMenu.Navigate.performed -= ChangePauseMenuSelection;
        playerControls.PauseMenu.Select.performed -= PauseMenuSelect;
        playerControls.PauseMenu.Cancel.performed -= CancelPauseMenu;
        playerControls.PauseMenu.Disable();
    }

    #endregion

    void Update() {
        transform.position += currentDir * _speed * Time.deltaTime;

        if (CurrentJob != null && CitySceneUIManager.Instance.CurrentGameMode == GameMode.Playing) {
            CurrentJob.ReduceTime(Time.deltaTime);

            if (CurrentJob.Expired) {
                // failed to deliver on time!
                CitySceneUIManager.Instance.ShowMessage($"Delivery overdue, you lose {CurrentJob.penalty}");
                EarnPenalty(CurrentJob.penalty);
                _currentJob = null;
                _spinner.gameObject.SetActive(false);
            }
            UpdateSpinner();
        }
        
        if (CurrentCash <= 0) {
            GameOver();
        }
    }

    private void UpdateSpinner() {
        _spinner.gameObject.SetActive(true);

        Vector2 origin = transform.position;
        Vector2 destination = _pickedupPackage ? GetBuilding(_currentJob.destination).transform.position : GetBuilding(_currentJob.origin).transform.position;

        float angle = Vector2.SignedAngle(destination, origin) + 180f;

        _spinner.transform.rotation = Quaternion.Euler(0, 0, -angle);
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
                    CitySceneUIManager.Instance.ShowMessage("Here's the package!", CurrentJob.origin.customer.customerName);
                    AudioManager.Instance.PlayPickup(CurrentJob.origin.customer.customerGender);
                    HidePickupMarker();
                    PutDeliveryMarket(GetBuilding(CurrentJob.destination));
                    _pickedupPackage = true;
                } else {
                    CitySceneUIManager.Instance.ShowMessage("Sorry, I don't have anything for you", currentBuilding.building.customer.customerName);
                }
            } else {
                // have a package for delivery
                if (currentBuilding.building == CurrentJob.destination) {
                    CitySceneUIManager.Instance.ShowMessage($"Thanks! Here's your ${CurrentJob.reward}!", CurrentJob.destination.customer.customerName);
                    AudioManager.Instance.PlayThanks(CurrentJob.destination.customer.customerGender);
                    HideDeliveryMarker();
                    EarnMoney(CurrentJob.reward);
                    AudioManager.Instance.PlayThanks(CurrentJob.destination.customer.customerGender);
                    _pickedupPackage = false;
                    _currentJob = null;
                    _spinner.gameObject.SetActive(false);
                } else {
                    CitySceneUIManager.Instance.ShowMessage("Sorry, I'm not expecting any deliveries", currentBuilding.building.customer.customerName);
                    JobManager.Instance.CreateJobs();
                }
            }
        } else {
            CitySceneUIManager.Instance.ShowMessage("Nobody's home...", "");
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

    // ----- NOTICEBOARD -----
    private void CloseNoticeboard() {
        // Check conditions to allow closing of noticeboard
        if (CitySceneUIManager.Instance.CurrentGameMode != GameMode.Noticeboard) return;

        // close the noticeboard
        NoticeboardManager.Instance.Close();
        CitySceneUIManager.Instance.CurrentGameMode = GameMode.Playing;
        DisableNoticeboardControls();
        EnablePlayerMovement();
        UpdateClaimJobBackgrounds();
        _currentClaimJob = 0;
    }

    private void OpenNoticeboard() {
        // only allow noticeboard to open if in play mode and no job selected
        if (CitySceneUIManager.Instance.CurrentGameMode != GameMode.Playing || _currentJob != null)
            return;

        // Open the Noticeboard
        NoticeboardManager.Instance.Open();
        CitySceneUIManager.Instance.CurrentGameMode = GameMode.Noticeboard;
        EnableNoticeboardControls();
        DisablePlayerMovement();
    }

    private void UpdateClaimJobBackgrounds() {
        if (_currentClaimJob == 0) {
            claimJob1.sprite = selected;
            claimJob2.sprite = unselected;
        } else {
            claimJob1.sprite = unselected;
            claimJob2.sprite = selected;
        }
    }

    private void ChangeNoticeboardSelection(InputAction.CallbackContext context) {
        float dir = context.action.ReadValue<float>();
        if (dir == 0) { return; }

        _currentClaimJob = _currentClaimJob == 0 ? 1 : 0;
        UpdateClaimJobBackgrounds();
    }

    private void CancelNoticeboard(InputAction.CallbackContext context) {
        CloseNoticeboard();
    }

    private void ToggleNoticeboard(InputAction.CallbackContext context) {        
        if (NoticeboardManager.Instance.IsOpen) {
            CloseNoticeboard();
        } else {
            OpenNoticeboard();
        }
    }

    public void ClaimJob(InputAction.CallbackContext context) {
        _currentJob = JobManager.Instance.jobs[_currentClaimJob];
        _pickedupPackage = false;
        JobManager.Instance.JobClaimed(CurrentJob);

        CloseNoticeboard();
        PutPickupMarker(GetBuilding(_currentJob.origin));
    }

    // ----- JOB MARKERS -----
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

    // ----- PAUSE MENU -----
    private void OpenPauseMenu() {
        if (CitySceneUIManager.Instance.CurrentGameMode != GameMode.Playing) return;
        pauseMenuAnim.SetTrigger("PauseMenuOpen");
        pauseMenuOpen = true;
        AudioManager.Instance.PlaySwoosh();
        CitySceneUIManager.Instance.CurrentGameMode = GameMode.PauseMenu;
        _currentPauseMenu = 0;
        DisablePlayerMovement();
        EnablePauseMenuControls();
    }

    private void ClosePauseMenu() {
        if (CitySceneUIManager.Instance.CurrentGameMode != GameMode.PauseMenu) return;
        pauseMenuAnim.SetTrigger("PauseMenuClose");
        pauseMenuOpen = false;
        AudioManager.Instance.PlaySwoosh();
        CitySceneUIManager.Instance.CurrentGameMode = GameMode.Playing;
        EnablePlayerMovement();
        DisablePauseMenuControls();
    }

    private void TogglePauseMenu(InputAction.CallbackContext context) {
        if (pauseMenuOpen) {
            ClosePauseMenu();
        } else {
            OpenPauseMenu();
        }
    }

    private void ChangePauseMenuSelection(InputAction.CallbackContext context) {
        float dir = context.action.ReadValue<float>();
        if (dir == 0) { return; }

        if (dir > 0) {
            // down
            _currentPauseMenu--;
            if (_currentPauseMenu < 0) { _currentPauseMenu = 2; }
        } else {
            // up
            _currentPauseMenu++;
            if (_currentPauseMenu > 2) { _currentPauseMenu = 0; }
        }
        UpdatePauseMenuButtons();
    }

    private void UpdatePauseMenuButtons() {
        resumeGame.sprite = _currentPauseMenu == 0 ? selected : unselected;
        returnToMain.sprite = _currentPauseMenu == 1 ? selected : unselected;
        exitGame.sprite = _currentPauseMenu == 2 ? selected : unselected;
    }

    private void PauseMenuSelect(InputAction.CallbackContext context) {
        switch (_currentPauseMenu) {
            case 0:                
                ResumeGame();
                break;
            case 1:
                ReturnToMain();
                break;
            case 2:
                CitySceneUIManager.Instance.ExitGame();
                break;
            default:
                Debug.Log($"Unknown pause menu option {_currentPauseMenu}");
                break;
        }
    }

    private void CancelPauseMenu(InputAction.CallbackContext context) {
        ResumeGame();
    }

    public void ResumeGame() {
        DisablePauseMenuControls();
        EnablePlayerMovement();
        ClosePauseMenu();
    }

    public void SetCash(int amount) {
        _currentCash = Mathf.Abs(amount);
        cashChanged?.Invoke();
    }

    public void EarnMoney(int amount) {
        _currentCash += amount;
        cashChanged?.Invoke();
    }

    public void EarnPenalty(int amount) {
        _currentCash -= Mathf.Abs(amount);
        cashChanged?.Invoke();
    }


    // GameOver
    public void GameOver() {
        gameOverAnim.SetTrigger("GameOver");
        //GameOverManager.Instance.Open();
        playerControls.MainMenu.Select.performed += GameOverExit;
        playerControls.MainMenu.Enable();
        playerControls.Player.Disable();
    }

    private void GameOverExit(InputAction.CallbackContext context) {
        playerControls.MainMenu.Select.performed -= GameOverExit;
        playerControls.MainMenu.Disable();
        ReturnToMain();
    }

    public void ReturnToMain() {
        DisablePauseMenuControls();
        SceneManager.LoadScene(0);
    }
}
