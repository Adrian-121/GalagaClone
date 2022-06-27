using UnityEngine;
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
    [Inject] private ProjectileManager _projectileManager;

    private EnemyMovement _movement;
    private SpriteRenderer _renderer;
    private Animator _animator;
    private ProjectileCollisionDetector _collisionDetector;

    private GameConfig.EnemyConfig _config;

    private float _lastTimeSinceProjectileCheck;
    private IEnemyTarget _target;

    public bool IsAlive { get; private set; }

    public void Construct(EnemyFormation formation) {
        _movement = GetComponentInChildren<EnemyMovement>();
        _movement.Construct(this, formation);

        _renderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();

        _collisionDetector = GetComponentInChildren<ProjectileCollisionDetector>();
        _collisionDetector.OnHit.AddListener(OnCollided);

        _renderer.gameObject.SetActive(false);
        IsAlive = false;
    }

    public void Initialize(TypeEnum type, MovementPatternResource movementPattern, Vector3 startPosition, float initialRotation, IEnemyTarget target) {
        _renderer.gameObject.SetActive(true);
        Configure(type);

        _movement.Initialize(movementPattern, _config, startPosition, initialRotation);
        _target = target;

        IsAlive = true;
    }

    public void Deinitialize(bool withBlast = false) {
        _movement.Deinitialize();
        _renderer.gameObject.SetActive(false);
        IsAlive = false;
    }

    private void Update() {
        if (!IsAlive) { return; }

        if (_target != null) {
            if (Time.time - _lastTimeSinceProjectileCheck > _config.ProjectileCheckTimeout) {
                _lastTimeSinceProjectileCheck = Time.time;
                if (Random.Range(0, 100) < _config.ProjectileChance) {
                    Vector3 vectorToTarget = _target.GetPosition() - transform.position;
                    Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 0) * vectorToTarget;
                    Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

                    _projectileManager.Fire(transform.position, targetRotation, gameObject);
                }
            }
        }        

        _movement.UpdateThis();
    }

    private void Configure(TypeEnum type) {
        _type = type;
        _config = _resourceLoader.GameConfig.GetEnemyConfigByType(_type);

        _hp = _config.hp;

        _animator.SetInteger(Constants.ANIM_ENEMY_TYPE, (int)_type);
        _animator.SetBool(Constants.ANIM_IS_FULL_HP, true);
    }

    public void TakeHit(GameObject from, bool fullDamage) {
        _hp = fullDamage ? 0 : _hp - 1;
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

    private void OnCollided(GameObject withObject) {
        ITakeHit takeHit = withObject.transform.GetComponentInParent<ITakeHit>();

        if (takeHit == null) { return; }
        if (withObject.transform.parent.gameObject == gameObject) { return; }

        takeHit.TakeHit(gameObject, false);
        TakeHit(withObject, true);
    }

    public class Factory : PlaceholderFactory<EnemyMainController> { }

}