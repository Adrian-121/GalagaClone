using UnityEngine;
using Zenject;

public class Projectile : MonoBehaviour, IGameControlled {

    [SerializeField] private float _speed = 5;
    [SerializeField] private float _lifetimeInSeconds = 3;

    private bool _isAlive;
    public bool IsAlive => _isAlive;

    // Components.
    private SpriteRenderer _renderer;
    private ProjectileCollisionDetector _collisionDetector;
    private GameObject _parent;
    private TrailRenderer _trail;

    private float _timeSinceLaunched;

    public void Construct() {
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _trail = GetComponentInChildren<TrailRenderer>();
        
        _collisionDetector = GetComponentInChildren<ProjectileCollisionDetector>();
        _collisionDetector.OnHit.AddListener(OnCollided);

        DestroyThis();
    }

    public void Initialize(Vector3 position, Quaternion rotation, GameObject parent) {
        transform.position = position;
        transform.rotation = rotation;
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);

        _parent = parent;

        _renderer.gameObject.SetActive(true);
        _trail.Clear();

        _isAlive = true;
        _timeSinceLaunched = Time.time;
    }

    public void Deinitialize() {
        DestroyThis();
    }

    public void OnUpdate() {
        if (!_isAlive) { return; }
     
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (Time.time - _timeSinceLaunched > _lifetimeInSeconds) {
            DestroyThis();
        }
    }

    private void DestroyThis() {
        _isAlive = false;
        _renderer.gameObject.SetActive(false);
    }

    private void OnCollided(GameObject withObject) {
        ITakeHit takeHit = withObject.transform.GetComponentInParent<ITakeHit>();

        // Prevent collisions with objects that don't matter.
        if (takeHit == null) { return; }
        // Prevent self collision.
        if (withObject.transform.parent.gameObject == _parent) { return; }
        // Prevent collision with the same object types. E.g. projectiles from enemies with other enemies.
        if (withObject.layer == _parent.layer) { return; }

        takeHit.TakeHit(gameObject, false);
        DestroyThis();
    }

    public class Factory : PlaceholderFactory<Projectile> { }
}