using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ProjectileManager : MonoBehaviour {

    private Projectile.Factory _projectileFactory;

    [Inject]
    public void Construct(Projectile.Factory projectileFactory) {
        _projectileFactory = projectileFactory;
    }

    public void Fire(Vector3 position, Quaternion rotation) {
        Projectile newProjectile = _projectileFactory.Create();
        newProjectile.transform.SetParent(transform);
        newProjectile.Initialize(position, rotation);
    }
    
}