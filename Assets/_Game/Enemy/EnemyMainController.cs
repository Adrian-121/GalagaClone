using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class EnemyMainController : MonoBehaviour, ITakeHit {

    public enum TypeEnum {
        GRUNT = 1,
        ESCORT = 2,
        BOSS = 3
    }

    private TypeEnum _type;
    public TypeEnum Type => _type;
    private float _hp = 1;

    [Inject] private SignalBus _signalBus;
    [Inject] private ResourceLoader _resourceLoader;
    [Inject] private VFXManager _vfxManager;
    [Inject] private SoundManager _soundManager;

    private EnemyMovement _movement;
    private SpriteRenderer _renderer;
    private Animator _animator;

    private GameConfig.EnemyConfig _config;
    public bool IsAlive { get; private set; }

    public void Construct(EnemyFormation formation) {
        _movement = GetComponentInChildren<EnemyMovement>();
        _movement.Construct(this, formation);

        _renderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();

        Deinitialize();
    }

    public void Initialize(TypeEnum type, MovementPatternResource movementPattern, Vector3 startPosition) {
        _renderer.gameObject.SetActive(true);
        Configure(type);

        _movement.Initialize(movementPattern, _config, startPosition);

        IsAlive = true;
    }

    public void Deinitialize(bool withBlast = false) {
        IsAlive = false;
        _renderer.gameObject.SetActive(false);
    }

    private void Update() {
        if (!IsAlive) { return; }

        _movement.UpdateThis();
    }

    private void Configure(TypeEnum type) {
        _type = type;
        _config = _resourceLoader.GameConfig.GetEnemyConfigByType(_type);

        _hp = _config.hp;

        _animator.SetInteger(Constants.ANIM_ENEMY_TYPE, (int)_type);
        _animator.SetBool(Constants.ANIM_IS_FULL_HP, true);
    }

    public void TakeHit(GameObject from) {
        _hp--;
        _animator.SetBool(Constants.ANIM_IS_FULL_HP, false);

        if (_hp <= 0) {
            _vfxManager.SpawnVFX(VFXObject.TypeEnum.ENEMY_EXPLOSION, transform.position);

            if (_type == TypeEnum.BOSS) {
                _soundManager.PlayBigBoom();
            }
            else {
                _soundManager.PlaySmallBoom();
            }
            
            _signalBus.Fire(new EnemyKilledSignal() { Points = _config.points });
            Deinitialize(true);
        }
    }

    public class Factory : PlaceholderFactory<EnemyMainController> { }

}