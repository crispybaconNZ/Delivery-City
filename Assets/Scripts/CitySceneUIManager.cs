using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode {
    Playing,        // wandering around the city
    Noticeboard,    // looking at the noticeboard
    PauseMenu,      // looking at the pause menu
    Interaction     // interacting with a customer
}

public class CitySceneUIManager : MonoBehaviour {
    public static CitySceneUIManager Instance;

    private StreetSignManager streetSign;
    [SerializeField] private List<JobListing> jobListings;
    [SerializeField] private Text currentCash;
    [SerializeField] private GameObject MessageBox;

    private GameMode gameMode;
    public GameMode CurrentGameMode { get { return gameMode; } set { gameMode = value; } }

    private void Awake() {
        if (Instance == null) { Instance = this; }
    }

    void Start() {
        PlayerMovement.Instance.onEnterBuildingZone.AddListener(OnPlayerEnterBuilding);
        PlayerMovement.Instance.onLeaveBuildingZone.AddListener(OnPlayerLeaveBuilding);
        PlayerMovement.Instance.cashChanged.AddListener(OnCashChanged);

        JobManager.Instance.jobChangeEvent.AddListener(OnJobListingChange);
        streetSign = StreetSignManager.Instance;

        streetSign.Hide();
        gameMode = GameMode.Playing;
        MessageBox.SetActive(false);
    }

    private void OnPlayerEnterBuilding(BuildingSO building) {
        // Update the street sign UI item with building name or address
        string sign = building.buildingName != "" ? building.buildingName : building.address;
        streetSign.SetAddress(sign);
        // Turn on the street sign UI item        
        streetSign.Show();
    }

    private void OnPlayerLeaveBuilding(BuildingSO building) {
        streetSign.Hide();
    }

    private void OnCashChanged() {
        currentCash.text = $"Cash: ${PlayerMovement.Instance.CurrentCash}";
    }

    private void OnJobListingChange(List<Job> jobs) {
        foreach (JobListing job in jobListings) { job.enabled = false; }

        if (jobs.Count > 0) {
            jobListings[0].UpdateListing(jobs[0]);
            jobListings[0].enabled = true;
        }

        if (jobs.Count > 1) {
            jobListings[1].UpdateListing(jobs[1]);
            jobListings[1].enabled = true;
        } 
    }

    public void ExitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

    private IEnumerator AutohideMessageBox(int time) {
        MessageBox.SetActive(true);
        yield return new WaitForSeconds(time);

        MessageBox.SetActive(false);
    }
    public void ShowMessage(string message) {
        MessageBox.GetComponentInChildren<Text>().text = message.Trim();
        StartCoroutine(AutohideMessageBox(3));
    }
}
