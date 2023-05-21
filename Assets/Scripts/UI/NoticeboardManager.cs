using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeboardManager : MonoBehaviour {
    public static NoticeboardManager Instance;
    [SerializeField] private float _timeToOpen;
    private bool _isOpen = false;

    public bool IsOpen { get { return _isOpen; } }

    private void Awake() {
        if (Instance == null) { Instance = this; }
        _isOpen = false;
    }

    public void Open() {
        AudioManager.Instance.PlaySwoosh();
        transform.LeanMoveLocalY(0f, _timeToOpen);
        _isOpen = true;
    }

    public void Close() {
        AudioManager.Instance.PlaySwoosh();
        transform.LeanMoveLocalY(-388f, _timeToOpen);
        _isOpen = false;
    }

    public void ToggleNoticeboard() {
        if (_isOpen) {
            Close();
        } else {
            Open();
        }
    }
}
