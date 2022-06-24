using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    public enum MovementTypes {
        FORMATION = 1,
        PATTERN = 2
    }

    private EnemyMainController _enemy;

    private BaseEnemyMover _currentMover;
    private EnemyFormationMover _formationMover;
    private EnemyPatternMover _patternMover;

    public void Construct(EnemyMainController enemy, EnemyFormation formation, float speed) {
        _enemy = enemy;

        _formationMover = GetComponentInChildren<EnemyFormationMover>();
        _formationMover.Construct(enemy.transform, formation, speed);

        _patternMover = GetComponentInChildren<EnemyPatternMover>();
        _patternMover.Construct(enemy.transform, speed);
        _patternMover.OnPatternFinished.AddListener(OnPatternFinished);
    }

    public void Initialize(Vector3 startPosition, MovementPatternResource movementPattern) {
        _enemy.transform.position = startPosition;
        _patternMover.Initialize(movementPattern);

        ChangeMovement(MovementTypes.PATTERN);
    }

    public void UpdateThis() {
        if (_currentMover != null) {
            _currentMover.OnUpdate();
        }
    }

    private void ChangeMovement(MovementTypes movementType) {
        if (_currentMover != null) {
            _currentMover.OnExit();
        }

        switch (movementType) {
            case MovementTypes.FORMATION:
                _currentMover = _formationMover;
                break;

            case MovementTypes.PATTERN:
                _currentMover = _patternMover;
                break;
        }

        if (_currentMover != null) {
            _currentMover.OnEnter();
        }
    }

    private void OnPatternFinished() {
        ChangeMovement(MovementTypes.FORMATION);
    }
}