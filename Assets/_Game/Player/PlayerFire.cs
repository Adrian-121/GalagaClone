using UnityEngine;
using Zenject;

public class PlayerFire : MonoBehaviour {

    private SignalBus _signalBus;

    private PlayerMainController _player;
    private ProjectileManager _projectileManager;
    private SoundManager _soundManager;

    private float _reloadTime = 0.5f;
    private float _timeFromLastShot;

    public void Construct(SignalBus signalBus, PlayerMainController player, ProjectileManager projectileManager, SoundManager soundManager) {
        _signalBus = signalBus;
        _signalBus.Subscribe<PlayerFireSignal>(OnPlayerFire);

        _player = player;

        _projectileManager = projectileManager;
        _soundManager = soundManager;
    }

    public void OnDestroy() {
        _signalBus.Unsubscribe<PlayerFireSignal>(OnPlayerFire);
    }

    private void OnPlayerFire() {
        if (Time.time - _timeFromLastShot > _reloadTime) {
            _timeFromLastShot = Time.time;
            _projectileManager.Fire(_player.transform.position, _player.transform.rotation, _player.gameObject);
            _soundManager.PlayShootSound();
        }
    }
}