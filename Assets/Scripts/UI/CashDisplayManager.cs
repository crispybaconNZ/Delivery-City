using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CashDisplayManager : MonoBehaviour {
    private AudioSource _audioSource;
    [SerializeField] private Text _cashTextField;
    private int _currentAmount = 1;

    [Header("Audio")]
    [SerializeField] private AudioClip _cashUp;
    [SerializeField] private AudioClip _cashDown;

    // Start is called before the first frame update
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        _currentAmount = PlayerMovement.Instance.CurrentCash;
        _cashTextField.text = $"Cash: ${PlayerMovement.Instance.CurrentCash}";
    }

    public void UpdateCashDisplay() {
        _cashTextField.text = $"Cash: ${PlayerMovement.Instance.CurrentCash}";

        if (PlayerMovement.Instance.CurrentCash > _currentAmount) {
            // cash has been gained
            _audioSource.clip = _cashUp;
        } else if (PlayerMovement.Instance.CurrentCash < _currentAmount) {
            // cash has been lost
            _audioSource.clip = _cashDown;            
        }
        _currentAmount = PlayerMovement.Instance.CurrentCash;
        _audioSource.Play();
    }
}
