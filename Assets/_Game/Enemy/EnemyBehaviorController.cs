using UnityEngine;

public class EnemyBehaviorController : MonoBehaviour {

    private EnemyMainController _enemy;

    private BaseEnemyBehavior _currentMover;
    private EnemyFormationBehavior _formationMover;
    private EnemyPatternBehavior _patternMover;

    public void Construct(EnemyMainController enemy, EnemyFormation formation) {
        _enemy = enemy;

        _formationMover = GetComponentInChildren<EnemyFormationBehavior>();
        _formationMover.Construct(enemy, formation);

        _patternMover = GetComponentInChildren<EnemyPatternBehavior>();
        _patternMover.Construct(enemy);
        _patternMover.OnPatternFinished.AddListener(OnPatternFinished);
    }

    public void Initialize(MovementPatternResource movementPattern, GameConfig.EnemyConfig config, Vector3 startPosition, float initialRotation) {
        _enemy.transform.position = startPosition;
        
        // Set the rotation, but reset it initially.
        _enemy.transform.rotation = Quaternion.identity;
        _enemy.transform.Rotate(Vector3.forward, initialRotation);

        _patternMover.Initialize(movementPattern, config);
        _formationMover.Initialize(config);

        ChangeMovement(BaseEnemyBehavior.TypeEnum.PATTERN);
    }

    public void Deinitialize() {
        // Clear the current mover.
        _currentMover = null;

        _patternMover.OnExit();
        _formationMover.OnExit();
    }

    public void OnUpdate() {
        if (_currentMover != null) {
            _currentMover.OnUpdate();
        }
    }

    private void ChangeMovement(BaseEnemyBehavior.TypeEnum movementType) {
        if (_currentMover != null) {
            _currentMover.OnExit();
        }

        switch (movementType) {
            case BaseEnemyBehavior.TypeEnum.FORMATION:
                _currentMover = _formationMover;
                break;

            case BaseEnemyBehavior.TypeEnum.PATTERN:
                _currentMover = _patternMover;
                break;
        }

        if (_currentMover != null) {
            _currentMover.OnEnter();
        }
    }

    private void OnPatternFinished() {
        ChangeMovement(BaseEnemyBehavior.TypeEnum.FORMATION);
    }
}