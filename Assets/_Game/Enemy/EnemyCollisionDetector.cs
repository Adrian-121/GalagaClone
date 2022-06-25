using UnityEngine;
using UnityEngine.Events;

public class EnemyCollisionDetector : MonoBehaviour {

    public UnityEvent OnHit;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer(Constants.PROJECTILE_LAYER)) {
            OnHit.Invoke();
        }        
    }

}