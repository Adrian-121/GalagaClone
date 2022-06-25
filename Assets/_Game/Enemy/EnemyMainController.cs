using UnityEngine;
using Zenject;

public class EnemyMainController : MonoBehaviour {

    public enum TypeEnum {
        GRUNT = 1,
        ESCORT = 2,
        BOSS = 3
    }

    private TypeEnum _type;
    private float _speed = 10;
    private float _hp = 1;

    [Inject]
    private ResourceLoader _resourceLoader;

    private EnemyMovement _movement;
    private EnemyCollisionDetector _collisionDetector;
    private SpriteRenderer _renderer;
    private Animator _animator;

    private GameConfig.EnemyConfig _config;
    public bool IsAlive { get; private set; }

    public void Construct(EnemyFormation formation) {
        _movement = GetComponentInChildren<EnemyMovement>();
        _movement.Construct(this, formation, _speed);

        _collisionDetector = GetComponentInChildren<EnemyCollisionDetector>();
        _collisionDetector.OnHit.AddListener(OnHit);

        _renderer = GetComponentInChildren<SpriteRenderer>();
        _renderer.gameObject.SetActive(false);

        _animator = GetComponentInChildren<Animator>();

        IsAlive = false;
    }

    public void Initialize(TypeEnum type, MovementPatternResource movementPattern) {
        Configure(type);

        _renderer.gameObject.SetActive(true);
        _movement.Initialize(movementPattern);

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
        _speed = _config.speed;

        _animator.SetInteger(Constants.ANIM_ENEMY_TYPE, (int)_type);
        _animator.SetBool(Constants.ANIM_IS_FULL_HP, true);
    }

    private void OnHit() {
        _hp--;
        _animator.SetBool(Constants.ANIM_IS_FULL_HP, false);

        if (_hp <= 0) {
            Deinitialize(true);
        }
    }

    public class Factory : PlaceholderFactory<EnemyMainController> { }

}