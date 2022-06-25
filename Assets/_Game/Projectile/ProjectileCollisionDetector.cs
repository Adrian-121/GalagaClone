using UnityEngine;
using UnityEngine.Events;

public class ProjectileCollisionDetector : MonoBehaviour {

    public UnityEvent<GameObject> OnHit;

    private void OnTriggerEnter2D(Collider2D collision) {
        OnHit.Invoke(collision.gameObject);
    }
}