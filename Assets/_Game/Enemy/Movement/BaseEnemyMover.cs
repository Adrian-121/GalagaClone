using UnityEngine;

public abstract class BaseEnemyMover : MonoBehaviour {

    protected Transform _enemyTransform;
    protected float _speed;

    //public virtual void Construct(Transform enemyTransform, float speed) {
    //    _enemyTransform = enemyTransform;
    //    _speed = speed;
    //}

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();

}