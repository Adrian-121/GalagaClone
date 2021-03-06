using UnityEngine;

/// <summary>
/// Handles the enemy's behavior once it has been assigned to the formation.
/// </summary>
public class EnemyFormationBehavior : BaseEnemyBehavior {

    protected EnemyFormation _enemyFormation;
    protected Vector2Int _formationSlot;

    private bool _isInFormation;

    public void Construct(Enemy enemy, EnemyFormation enemyFormation) {
        _enemy = enemy;
        _enemyFormation = enemyFormation;
    }

    public void Initialize(GameConfig.EnemyConfig config) {
        _config = config;
        _isInFormation = false;
    }

    public override void OnEnter() {
        if (_enemyFormation == null) { return; }

        _formationSlot = _enemyFormation.AssignEnemy(_enemy);
        _enemy.transform.rotation = Quaternion.identity;
    }

    public override void OnExit() {
        if (_enemyFormation == null) { return; }
        _enemyFormation.RemoveEnemy(_formationSlot);
    }

    public override void OnUpdate() {
        if (_enemyFormation == null) { return; }

        if (!_isInFormation) {
            GotoFormationPosition();
        }
        else {
            MaintainFormation();
        }        
    }

    /// <summary>
    /// Moves the enemy towards the designated formation position.
    /// </summary>
    private void GotoFormationPosition() {
        Vector3 formationPosition = _enemyFormation.FormationSlotToWorldPosition(_formationSlot);
        float dist = Vector3.Distance(_enemy.transform.position, formationPosition);

        if (dist < 0.01f) {
            _isInFormation = true;
            return;
        }

        Vector3 dir = (formationPosition - _enemy.transform.position).normalized;
        _enemy.transform.Translate(_config.speed * dir * Time.deltaTime);
    }

    /// <summary>
    /// Keeps the enemy in the formation position.
    /// </summary>
    private void MaintainFormation() {
        _enemy.transform.position = _enemyFormation.FormationSlotToWorldPosition(_formationSlot);
    }
}