using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SoundSystem : MonoBehaviour {

    [SerializeField] private Vector2 _pitch;

    [SerializeField] private List<AudioClip> _shootingClipList;
    [SerializeField] private AudioClip _uiButtonPressedClip;
    [SerializeField] private AudioClip _smallBoom;
    [SerializeField] private AudioClip _bigBoom;

    private SignalBus _signalBus;
    private AudioSource _audioSource;

    [Inject]
    public void Construct(SignalBus signalBus) {
        _audioSource = GetComponent<AudioSource>();

        _signalBus = signalBus;
        _signalBus.Subscribe<SoundMuteButtonSignal>(x => {
            _audioSource.mute = !_audioSource.mute;
            _signalBus.Fire(new SoundMuteStatusChangedSignal() { IsMuted = _audioSource.mute });
        });

        _signalBus.Subscribe<UIButtonPressedSignal>(x => {
            _audioSource.pitch = Random.Range(_pitch.x, _pitch.y);
            _audioSource.PlayOneShot(_uiButtonPressedClip);
        });
    }

    public void PlayShootSound() {
        _audioSource.pitch = Random.Range(_pitch.x, _pitch.y);
        _audioSource.PlayOneShot(_shootingClipList[Random.Range(0, _shootingClipList.Count-1)]);        
    }

    public void PlaySmallBoom() {
        _audioSource.pitch = Random.Range(_pitch.x, _pitch.y);
        _audioSource.PlayOneShot(_smallBoom);
    }

    public void PlayBigBoom() {
        _audioSource.pitch = Random.Range(_pitch.x, _pitch.y);
        _audioSource.PlayOneShot(_bigBoom);
    }
}