using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ProjectileManager : MonoBehaviour {

    private Projectile.Factory _projectileFactory;
    private List<Projectile> _projectileList = new List<Projectile>();

    [Inject]
    public void Construct(Projectile.Factory projectileFactory) {
        _projectileFactory = projectileFactory;

        for (int i = 0; i < Constants.PROJECTILE_POOL_MAX; i++) {
            Projectile newProjectile = _projectileFactory.Create();
            newProjectile.transform.SetParent(transform);

            _projectileList.Add(newProjectile);
        }
    }

    public void Fire(Vector3 position, Quaternion rotation, GameObject parent) {
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