using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IGameControlled {

    private float _screenBoundsOffset = 0.5f;

    private PlayerInput _playerInput;
    private ResourceLoader _resourceLoader;

    public void Construct(PlayerInput playerInput, ResourceLoader resourceLoader) {
        _playerInput = playerInput;
        _resourceLoader = resourceLoader;
    }

    public void Deinitialize() {
    }

    public void OnUpdate() {
        Gamepad gamepad = Gamepad.current;

        Vector2 move = gamepad.leftStick.ReadValue();
        move.x = Mathf.RoundToInt(move.x);

        // If no input from the controller, check the keyboard. (Used for debug)
        if (move.magnitude <= 0) {
            move = _playerInput.actions["Move"].ReadValue<Vector2>();
        }

        transform.parent.Translate(move * _resourceLoader.GameConfig.PlayerMoveSpeed * Vector2.right * Time.deltaTime);

        Vector3 minScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 maxScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        float posX = Mathf.Clamp(transform.parent.position.x, minScreenBounds.x + _screenBoundsOffset, maxScreenBounds.x - _screenBoundsOffset);
        transform.parent.position = new Vector2(posX, transform.parent.position.y);
    }
}