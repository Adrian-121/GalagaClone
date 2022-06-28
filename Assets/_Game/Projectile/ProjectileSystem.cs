using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ProjectileSystem : MonoBehaviour, IGameControlled {

    private Projectile.Factory _projectileFactory;
    private List<Projectile> _projectileList = new List<Projectile>();

    [Inject]
    public void Construct(Projectile.Factory projectileFactory) {
        _projectileFactory = projectileFactory;

        // Filling the projectile pool.

        for (int i = 0; i < Constants.PROJECTILE_POOL_MAX; i++) {
            Projectile newProjectile = _projectileFactory.Create();

            newProjectile.transform.SetParent(transform);
            newProjectile.Construct();

            _projectileList.Add(newProjectile);
        }
    }

    public void Deinitialize() {
        foreach (Projectile projectile in _projectileList) {
            projectile.Deinitialize();
        }
    }

    public void OnUpdate() {
        foreach (Projectile projectile in _projectileList) {
            projectile.OnUpdate();
        }
    }

    /// <summary>
    /// Tries to fire a projectile, if any available.
    /// </summary>
    public void TryFire(Vector3 position, Quaternion rotation, GameObject parent) {
        Projectile projectileToUse = null;

        foreach (Projectile projectile in _projectileList) {
            if (!projectile.IsAlive) {
                projectileToUse = projectile;
                break;
            }
        }

        if (projectileToUse != null) {
            projectileToUse.Initialize(position, rotation, parent);
        }
    }
}