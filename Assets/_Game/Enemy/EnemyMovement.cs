using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    private EnemyMainController _enemy;

    private BaseEnemyMover _currentMover;
    private EnemyFormationMover _formationMover;
    private EnemyPatternMover _patternMover;
    private EnemyLungeMover _lungeMover;

    public void Construct(EnemyMainController enemy, EnemyFormation formation) {
        _enemy = enemy;

        _formationMover = GetComponentInChildren<EnemyFormationMover>();
        _formationMover.Construct(enemy, formation);

        _patternMover = GetComponentInChildren<EnemyPatternMover>();
        _patternMover.Construct(enemy);
        _patternMover.OnPatternFinished.AddListener(OnPatternFinished);

        _lungeMover = GetComponentInChildren<EnemyLungeMover>();
        _lungeMover.Construct(enemy);
        _lungeMover.OnLungeFinished.AddListener(OnLungeFinished);
    }

    public void Initialize(MovementPatternResource movementPattern, GameConfig.EnemyConfig config, Vector3 startPosition, float initialRotation) {
        _enemy.transform.position = startPosition;
        
        // Set the rotation, but reset it initially.
        _enemy.transform.rotation = Quaternion.identity;
        _enemy.transform.Rotate(Vector3.forward, initialRotation);

        _patternMover.Initialize(movementPattern, config);
        _formationMover.Initialize(config);

        ChangeMovement(BaseEnemyMover.TypeEnum.PATTERN);
    }

    public void Deinitialize() {
        _patternMover.OnExit();
        _formationMover.OnExit();
    }

    public void OnUpdate() {
        if (_currentMover != null) {
            _currentMover.OnUpdate();
        }
    }

    private void ChangeMovement(BaseEnemyMover.TypeEnum movementType) {
        if (_currentMover != null) {
            _currentMover.OnExit();
        }

        switch (movementType) {
            case BaseEnemyMover.TypeEnum.FORMATION:
                _currentMover = _formationMover;
                break;

            case BaseEnemyMover.TypeEnum.PATTERN:
                _currentMover = _patternMover;
                break;
        }

        if (_currentMover != null) {
            _currentMover.OnEnter();
        }
    }

    private void OnPatternFinished() {
        ChangeMovement(BaseEnemyMover.TypeEnum.FORMATION);
    }

    private void OnLungeFinished() {
        ChangeMovement(BaseEnemyMover.TypeEnum.FORMATION);
    }
}