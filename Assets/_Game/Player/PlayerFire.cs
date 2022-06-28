using UnityEngine;
using Zenject;

public class PlayerFire : MonoBehaviour, IGameControlled {

    private SignalBus _signalBus;

    private PlayerMainController _player;
    private ProjectileManager _projectileManager;
    private SoundManager _soundManager;

    private float _reloadTime = 0.5f;
    private float _timeFromLastShot;

    public void Construct(SignalBus signalBus, PlayerMainController player, ProjectileManager projectileManager, SoundManager soundManager) {
        _signalBus = signalBus;
        _player = player;

        _projectileManager = projectileManager;
        _soundManager = soundManager;
    }

    public void Initialize() {
        _signalBus.Subscribe<PlayerFireSignal>(OnPlayerFire);
    }

    public void Deinitialize() {
        _signalBus.TryUnsubscribe<PlayerFireSignal>(OnPlayerFire);
    }

    public void OnUpdate() {
    }

    private void OnPlayerFire() {
        if (Time.time - _timeFromLastShot > _reloadTime) {
            _timeFromLastShot = Time.time;
            _projectileManager.TryFire(_player.transform.position, _player.transform.rotation, _player.gameObject);
            _soundManager.PlayShootSound();
        }
    }
}