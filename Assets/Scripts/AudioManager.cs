using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager Instance;
    private AudioSource _audioSource;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip _sadTrombone;
    [SerializeField] private AudioClip _swoosh;
    [SerializeField] private AudioClip _kaching;

    [Header("Voice Messages")]
    [SerializeField] private AudioClip[] _thanks;
    [SerializeField] private AudioClip[] _packagePickup;

    private void Awake() {
        if (Instance == null) { Instance = this; }
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySwoosh() {
        _audioSource.clip = _swoosh;
        _audioSource.Play();
    }

    public void PlaySadTrombone() {
        _audioSource.clip = _sadTrombone;
        _audioSource.Play();
    }

    public void PlayKaching() {
        _audioSource.clip = _kaching;
        _audioSource.Play();
    }

    public void PlayThanks(GenderEnum gender = GenderEnum.None) {
        int index = 0;
        if (gender == GenderEnum.None) {
            index = Random.Range(0, _thanks.Length);    // pick a random voice
        } else if (gender == GenderEnum.Female) {
            index = 0;
        } else {
            index = 1;
        }
        
        _audioSource.clip = _thanks[index];
        _audioSource.Play();
    }

    public void PlayPickup(GenderEnum gender = GenderEnum.None) {
        int index = 0;
        if (gender == GenderEnum.None) {
            index = Random.Range(0, _thanks.Length);    // pick a random voice
        } else if (gender == GenderEnum.Female) {
            index = 0;
        } else {
            index = 1;
        }

        _audioSource.clip = _packagePickup[index];
        _audioSource.Play();
    }
}
