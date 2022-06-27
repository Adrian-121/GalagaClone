using UnityEngine;
using Zenject;

public class Projectile : MonoBehaviour, IGameControlled {

    [SerializeField] private float _speed = 5;
    [SerializeField] private float _lifetimeInSeconds = 3;

    private bool _isAlive;
    public bool IsAlive => _isAlive;

    private SpriteRenderer _renderer;
    private ProjectileCollisionDetector _collisionDetector;
    private GameObject _parent;
    private TrailRenderer _trail;

    private float _timeSinceLaunched;

    private void Awake() {
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

        if (takeHit == null) { return; }
        if (withObject.transform.parent.gameObject == _parent) { return; }
        if (withObject.layer == _parent.layer) { return; }

        takeHit.TakeHit(gameObject, false);
        DestroyThis();
    }

    public class Factory : PlaceholderFactory<Projectile> { }
}