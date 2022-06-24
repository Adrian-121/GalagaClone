using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerMainController : MonoBehaviour {

    private SignalBus _signalBus;
    private PlayerInput _inputSystem;

    private ProjectileManager _projectileManager;

    private PlayerFire _playerFire;

    [Inject]
    public void Construct(ProjectileManager projectileManager, SignalBus signalBus) {
        Debug.Log(projectileManager);
        Debug.Log(signalBus);

        _projectileManager = projectileManager;
        _signalBus = signalBus;

        _playerFire = GetComponentInChildren<PlayerFire>();
        _playerFire.Construct(_signalBus, this, _projectileManager);
    }

    public class Factory : PlaceholderFactory<PlayerMainController> { }

}