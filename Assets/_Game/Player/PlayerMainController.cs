using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerMainController : MonoBehaviour, ITakeHit, IEnemyTarget, IGameControlled {

    // Game Systems.
    private PlayerInput _playerInput;
    private SignalBus _signalBus;
    private VFXManager _vfxManager;
    private SoundManager _soundManager;

    // Player Components.
    private PlayerMovement _playerMovement;
    private PlayerFire _playerFire;
    private SpriteRenderer _renderer;

    // Variables.
    private Vector3 _startPosition;
    private bool _isAlive;

    [Inject]
    public void Construct(ProjectileManager projectileManager, SignalBus signalBus, SoundManager soundManager, VFXManager vfxManager) {
        _signalBus = signalBus;
        _soundManager = soundManager;
        _vfxManager = vfxManager;

        _playerInput = GetComponent<PlayerInput>();

        _playerMovement = GetComponentInChildren<PlayerMovement>();
        _playerMovement.Construct(_playerInput);

        _playerFire = GetComponentInChildren<PlayerFire>();
        _playerFire.Construct(_signalBus, this, projectileManager, soundManager);

        _renderer = GetComponentInChildren<SpriteRenderer>();
        _isAlive = false;
    }

    public void Initialize(Vector3 startPosition) {
        _renderer.gameObject.SetActive(true);

        _playerFire.Initialize();

        _startPosition = startPosition;
        transform.position = startPosition;

        _isAlive = true;
    }

    public void Deinitialize() {
        _renderer.gameObject.SetActive(false);

        _playerFire.Deinitialize();

        _isAlive = false;
    }

    /// <summary>
    /// Called when the player collides with a projectile or an enemy.
    /// </summary>
    public void TakeHit(GameObject from, bool fullDamage) {
        _signalBus.Fire<PlayerObjectKilledSignal>();

        _vfxManager.TrySpawnVFX(VFXObject.TypeEnum.PLAYER_EXPLOSION, transform.position);
        _soundManager.PlayBigBoom();

        transform.position = _startPosition;
    }

    public void OnUpdate() {
        if (!_isAlive) { return; }

        _playerMovement.OnUpdate();
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public class Factory : PlaceholderFactory<PlayerMainController> { }
}