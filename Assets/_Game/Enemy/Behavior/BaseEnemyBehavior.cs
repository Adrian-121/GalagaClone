using UnityEngine;

/// <summary>
/// Base class used to implement all the enemy's behaviors
/// </summary>
public abstract class BaseEnemyBehavior : MonoBehaviour {

    public enum TypeEnum {
        FORMATION = 1,
        PATTERN = 2
    }

    [SerializeField] protected TypeEnum _type;
    public TypeEnum Type => _type;

    protected EnemyMainController _enemy;
    protected GameConfig.EnemyConfig _config;

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();

}