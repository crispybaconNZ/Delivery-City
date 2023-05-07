using UnityEngine;
using UnityEngine.UI;

public class JobListing : MonoBehaviour {
    public Text jobName;
    public Text locations;
    public Text reward;
    public Text penalty;
    public Text timeLimit;
    public Image image;
    public Button claimButton;
    private Job _job;

    public void UpdateListing(string jobName, string origin, string destination, int reward, int penalty, int timeLimit, Sprite image) {
        this.jobName.text = jobName;
        locations.text = $"{origin} to {destination}";
        this.reward.text = $"Reward: ${reward}";
        this.penalty.text = $"Penalty: -${Mathf.Abs(penalty)}";
        this.timeLimit.text = $"Time limit: {timeLimit} seconds";
        this.image.sprite = image;
    }

    public void UpdateListing(Job job) {
        jobName.text = job.job_name;
        string origin = job.origin.buildingName != "" ? job.origin.buildingName : job.origin.address;
        string destination = job.destination.buildingName != "" ? job.destination.buildingName : job.destination.address;
        locations.text = $"{origin} to {destination}";
        this.reward.text = $"Reward: ${job.reward}";
        this.penalty.text = $"Penalty: -${Mathf.Abs(job.penalty)}";
        this.timeLimit.text = $"Time limit: {job.time_limit} seconds";
        this.image.sprite = job.goods.image;
        this._job = job;

        //claimButton.enabled = PlayerMovement.Instance.CurrentJob == null;
        //claimButton.GetComponentInChildren<Text>().text = PlayerMovement.Instance.CurrentJob == null ? "Claim Job" : "Finish current job first";
    }

    public void ClaimJob() {
        Debug.Log("JobListing.ClaimJob()");
        JobManager.Instance.jobClaimedEvent?.Invoke(_job);
        this._job = null;
    }

    private void Update() {
        //claimButton.enabled = PlayerMovement.Instance.CurrentJob == null;
        //claimButton.GetComponentInChildren<Text>().text = PlayerMovement.Instance.CurrentJob == null ? "Claim Job" : "Finish current job first";
    }
}
