using UnityEngine;
using Zenject;

public class Projectile : MonoBehaviour {

    [SerializeField] private float _speed = 5;
    [SerializeField] private float _lifetimeInSeconds = 3;

    private bool _isAlive;
    public bool IsAlive => _isAlive;

    private SpriteRenderer _renderer;

    private void Awake() {
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Initialize(Vector3 position, Quaternion rotation) {
        transform.position = position;
        transform.rotation = rotation;

        _renderer.gameObject.SetActive(true);
        _isAlive = true;

        Invoke("DestroyThis", _lifetimeInSeconds);
    }

    private void Update() {
        if (!_isAlive) { return; }
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }

    private void DestroyThis() {
        _isAlive = false;
        _renderer.gameObject.SetActive(false);
    }

    public class Factory : PlaceholderFactory<Projectile> { }
}