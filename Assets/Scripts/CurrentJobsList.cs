using UnityEngine;
using UnityEngine.UI;

public class CurrentJobsList : MonoBehaviour {
    private Image _background;
    [SerializeField] private Text _headingText;
    [SerializeField] private Text _jobsList;
    [SerializeField] private Text _timerText;
    private Animator _anim;
    [SerializeField] private float _timeToOpen;

    public void Open() {
        transform.LeanScale(Vector2.one, _timeToOpen);
    }

    public void Close() {
        transform.LeanScale(Vector2.zero, _timeToOpen);
    }

    void Start() {
        _background = GetComponentInChildren<Image>();
        _anim = _timerText.GetComponent<Animator>();
        Hide();
    }

    void Hide() {
        _background.enabled = false;
        _headingText.enabled = false;
        _jobsList.enabled = false;
        _timerText.enabled = false;
        Close();
    }

    void Update() {
        // only show this list if the player actually has jobs
        bool showList = PlayerMovement.Instance.CurrentJob != null;
        if (!showList) {
            Hide();
            return;
        }

        Job job = PlayerMovement.Instance.CurrentJob;
        _jobsList.text = $"Deliver {job.goods.name}\n" +
            $"from {(job.origin.buildingName != "" ? job.origin.buildingName : job.origin.address)}\n" +
            $"to {(job.destination.buildingName != "" ? job.destination.buildingName : job.destination.address)}\n" +
            $"for ${job.reward};\n" +
            $"-${Mathf.Abs(job.penalty)} if not delivered on time";
        _timerText.text = $"{(int)job.time_limit} seconds";

        if ((int)job.time_limit <= 10) {
            _timerText.color = Color.red;
            _anim.SetBool("Wobble", true);
        } else {
            _timerText.color = GameManager.dialogColor;
            _anim.SetBool("Wobble", false);
        }       

        _background.enabled = showList;
        _headingText.enabled = showList;
        _jobsList.enabled = showList;
        _timerText.enabled = showList;
        Open();
    }
}
