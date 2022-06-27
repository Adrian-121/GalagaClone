using UnityEngine;

/// <summary>
/// Designates a game object as a possible enemy target.
/// </summary>
public interface IEnemyTarget {

    Vector3 GetPosition();

}