using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerMainController : MonoBehaviour, ITakeHit {

    private SignalBus _signalBus;
    private PlayerInput _inputSystem;

    private ProjectileManager _projectileManager;

    private PlayerFire _playerFire;

    [Inject]
    public void Construct(ProjectileManager projectileManager, SignalBus signalBus, SoundManager soundManager) {
        Debug.Log(projectileManager);
        Debug.Log(signalBus);

        _projectileManager = projectileManager;
        _signalBus = signalBus;

        _playerFire = GetComponentInChildren<PlayerFire>();
        _playerFire.Construct(_signalBus, this, projectileManager, soundManager);
    }

    public void TakeHit(GameObject from) {
        Debug.Log(" PLAYER TAKE HIT ");
    }

    public class Factory : PlaceholderFactory<PlayerMainController> { }

}