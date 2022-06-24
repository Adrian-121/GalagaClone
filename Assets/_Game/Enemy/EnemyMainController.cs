using UnityEngine;
using Zenject;

public class EnemyMainController : MonoBehaviour {

    [SerializeField] private float _speed = 10;

    private EnemyFormation _formation;

    private EnemyMovement _movement;

    private bool _isAlive = false;

    public void Construct(EnemyFormation formation) {
        _formation = formation;

        _movement = GetComponentInChildren<EnemyMovement>();
        _movement.Construct(this, formation, _speed);
    }

    public void Initialize(Vector3 startPosition, MovementPatternResource movementPattern) {
        _isAlive = true;

        _movement.Initialize(startPosition, movementPattern);
    }

    private void Update() {
        if (!_isAlive) { return; }

        _movement.UpdateThis();
    }

    public class Factory : PlaceholderFactory<EnemyMainController> { }

}