using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IGameControlled {

    [SerializeField] private float _speed = 5;

    private float _screenBoundsOffset = 0.5f;

    private PlayerInput _playerInput;

    public void Construct(PlayerInput playerInput) {
        _playerInput = playerInput;
    }

    public void Deinitialize() {

    }

    public void OnUpdate() {
        Gamepad gamepad = Gamepad.current;

        Vector2 move = gamepad.leftStick.ReadValue();
        move.x = Mathf.RoundToInt(move.x);

        if (move.magnitude <= 0) {
            move = _playerInput.actions["Move"].ReadValue<Vector2>();
        }

        transform.parent.Translate(move * _speed * Vector2.right * Time.deltaTime);

        Vector3 minScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 maxScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        transform.parent.position = new Vector2(
            Mathf.Clamp(transform.parent.position.x, minScreenBounds.x + _screenBoundsOffset, maxScreenBounds.x - _screenBoundsOffset), transform.parent.position.y);
    }
}