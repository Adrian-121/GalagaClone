using UnityEngine;
using UnityEngine.Events;

public class EnemyLungeMover : BaseEnemyMover {

    public UnityEvent OnLungeFinished;

    public void Construct(EnemyMainController enemy) {
        _enemy = enemy;
    }

    public override void OnEnter() {
    }

    public override void OnExit() {
    }

    public override void OnUpdate() {
    }
}
