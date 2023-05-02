using UnityEngine;
using UnityEngine.UI;

public class CurrentJobsList : MonoBehaviour {
    [SerializeField] private Text _headingText;
    [SerializeField] private Text _jobsList;

    void Start() {
        _headingText.enabled = false;
        _jobsList.enabled = false;
    }

    void Update() {
        // only show this list if the player actually has jobs
        bool showList = PlayerMovement.Instance.CurrentJob != null;
        if (!showList) {
            _headingText.enabled = false;
            _jobsList.enabled = false;
            return;
        }

        Job job = PlayerMovement.Instance.CurrentJob;
        _jobsList.text = $"Deliver {job.goods.name}\n" +
            $"from {(job.origin.buildingName != "" ? job.origin.buildingName : job.origin.address)}\n" +
            $"to {(job.destination.buildingName != "" ? job.destination.buildingName : job.destination.address)}\n" +
            $"for ${job.reward};\n" +
            $"-${Mathf.Abs(job.penalty)} if not delivered in\n" +
            $"{(int)job.time_limit} seconds";

        _headingText.enabled = showList;
        _jobsList.enabled = showList;
    }
}
