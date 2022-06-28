using UnityEngine;
using Zenject;

public class VFXObject : MonoBehaviour, IGameControlled {

    public enum TypeEnum {
        PLAYER_EXPLOSION = 1,
        ENEMY_EXPLOSION = 2
    }

    public bool IsAlive { get; private set; }

    [SerializeField] private float _lifetimeInSeconds = 5.15f;

    // Components.
    private SpriteRenderer _renderer;
    private Animator _animator;

    
    private float _timeSinceLaunched;

    public void Construct() {
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        DestroyThis();
    }

    public void Initialize(TypeEnum type, Vector3 position) {
        IsAlive = true;

        transform.position = position;

        _renderer.gameObject.SetActive(true);
        _animator.SetInteger(Constants.ANIM_EXPLOSION_TYPE, (int)type);
        
        _timeSinceLaunched = Time.time;
    }

    public void Deinitialize() {
        DestroyThis();
    }

    public void OnUpdate() {
        if (!IsAlive) { return; }

        if (Time.time - _timeSinceLaunched > _lifetimeInSeconds) {
            DestroyThis();
        }
    }

    private void DestroyThis() {
        _animator.SetInteger(Constants.ANIM_EXPLOSION_TYPE, 0);
        _renderer.gameObject.SetActive(false);
        IsAlive = false;
    }

    public class Factory : PlaceholderFactory<VFXObject> { }
}