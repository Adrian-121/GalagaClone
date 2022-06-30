using UnityEngine;
using Zenject;

public class PlayerFire : MonoBehaviour, IGameControlled {

    private SignalBus _signalBus;

    private Player _player;
    private ProjectileSystem _projectileManager;
    private SoundSystem _soundManager;
    private ResourceLoader _resourceLoader;

    private float _timeFromLastShot;

    public void Construct(SignalBus signalBus, Player player, ProjectileSystem projectileManager, SoundSystem soundManager, ResourceLoader resourceLoader) {
        _signalBus = signalBus;
        _player = player;
        
        _projectileManager = projectileManager;
        _soundManager = soundManager;
        _resourceLoader = resourceLoader;
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
        if (Time.time - _timeFromLastShot > _resourceLoader.GameConfig.PlayerReloadTime) {
            _timeFromLastShot = Time.time;
            _projectileManager.TryFire(_player.transform.position, _player.transform.rotation, _player.gameObject);
            _soundManager.PlayShootSound();
        }
    }
}