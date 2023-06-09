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
}
