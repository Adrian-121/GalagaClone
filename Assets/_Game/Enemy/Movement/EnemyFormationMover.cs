using UnityEngine;

public class EnemyFormationMover : BaseEnemyMover {

    protected EnemyFormation _enemyFormation;
    protected Vector2Int _formationSlot;

    private bool _isInFormation = false;

    public void Construct(Transform enemyTransform, EnemyFormation enemyFormation, float speed) {
        _enemyTransform = enemyTransform;
        _speed = speed;
        _enemyFormation = enemyFormation;
    }

    public override void OnEnter() {
        _formationSlot = _enemyFormation.GetFormationSlot();
    }

    public override void OnExit() {
        _enemyFormation.ReleaseFormationSlot(_formationSlot);
    }

    public override void OnUpdate() {
        if (!_isInFormation) {
            GotoFormationPosition();
        }
        else {
            MaintainFormation();
        }        
    }

    private void GotoFormationPosition() {
        Vector3 formationPosition = _enemyFormation.GetRelativePosition(_formationSlot);
        float dist = Vector3.Distance(_enemyTransform.position, formationPosition);

        if (dist < 0.01f) {
            _isInFormation = true;
            return;
        }

        Vector3 dir = (formationPosition - _enemyTransform.position).normalized;
        _enemyTransform.Translate(_speed * dir * Time.deltaTime);
    }

    private void MaintainFormation() {
        _enemyTransform.position = _enemyFormation.GetRelativePosition(_formationSlot);
    }
}