using UnityEngine;
using Zenject;

public class PlayerFire : MonoBehaviour, IGameControlled {

    private SignalBus _signalBus;

    private Player _player;
    private ProjectileSystem _projectileManager;
    private SoundSystem _soundManager;

    private float _reloadTime = 0.5f;
    private float _timeFromLastShot;

    public void Construct(SignalBus signalBus, Player player, ProjectileSystem projectileManager, SoundSystem soundManager) {
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