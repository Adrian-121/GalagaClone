using UnityEngine;

public class EnemyBehaviorController : MonoBehaviour {

    private Enemy _enemy;

    private BaseEnemyBehavior _currentBehavior;
    private EnemyFormationBehavior _formationBehavior;
    private EnemyPatternBehavior _patternBehavior;

    public void Construct(Enemy enemy, EnemyFormation formation) {
        _enemy = enemy;

        _formationBehavior = GetComponentInChildren<EnemyFormationBehavior>();
        _formationBehavior.Construct(enemy, formation);

        _patternBehavior = GetComponentInChildren<EnemyPatternBehavior>();
        _patternBehavior.Construct(enemy);
        _patternBehavior.OnPatternFinished.AddListener(OnPatternFinished);
    }

    public void Initialize(MovementPatternResource movementPattern, GameConfig.EnemyConfig config, Vector3 startPosition, float initialRotation) {
        _enemy.transform.position = startPosition;
        
        // Set the rotation, but reset it initially.
        _enemy.transform.rotation = Quaternion.identity;
        _enemy.transform.Rotate(Vector3.forward, initialRotation);

        _patternBehavior.Initialize(movementPattern, config);
        _formationBehavior.Initialize(config);

        // Sets the starting behavior to pattern.
        ChangeBehavior(BaseEnemyBehavior.TypeEnum.PATTERN);
    }

    public void Deinitialize() {
        // Clear the current mover.
        _currentBehavior = null;

        _patternBehavior.OnExit();
        _formationBehavior.OnExit();
    }

    public void OnUpdate() {
        if (_currentBehavior != null) {
            _currentBehavior.OnUpdate();
        }
    }

    private void ChangeBehavior(BaseEnemyBehavior.TypeEnum movementType) {
        if (_currentBehavior != null) {
            _currentBehavior.OnExit();
        }

        switch (movementType) {
            case BaseEnemyBehavior.TypeEnum.FORMATION:
                _currentBehavior = _formationBehavior;
                break;

            case BaseEnemyBehavior.TypeEnum.PATTERN:
                _currentBehavior = _patternBehavior;
                break;
        }

        if (_currentBehavior != null) {
            _currentBehavior.OnEnter();
        }
    }

    private void OnPatternFinished() {
        ChangeBehavior(BaseEnemyBehavior.TypeEnum.FORMATION);
    }
}