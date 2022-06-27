using System.Collections.Generic;
using UnityEngine;

public class EnemyFormationMovement : MonoBehaviour, IGameControlled {

    public enum MovementTypeEnum {
        SIDEWAYS = 1,
        IN_OUT = 2,
        IDLE = 3,
    }

    private MovementTypeEnum _currentMovement = MovementTypeEnum.SIDEWAYS;
    private float _currentMovementTime;

    private FormationPatternResource _formationResource;
    private GameConfig.FormationMovementConfig _selectedFormationConfig;

    private float _direction = -1;
    private float _positionOffset = 2.5f;

    private float _rightMargin;
    private float _leftMargin;

    private Transform _formationTransform;
    private List<GameConfig.FormationMovementConfig> _formationConfigList;

    public void Construct(Transform formationTransform, List<GameConfig.FormationMovementConfig> formationConfigList) {
        _formationTransform = formationTransform;        
        _formationConfigList = formationConfigList;        
    }

    public void Initialize(FormationPatternResource formationResource) {
        _formationResource = formationResource;
        ChangeCurrentMovement(MovementTypeEnum.SIDEWAYS);
    }

    public void Deinitialize() {
    }

    public void OnUpdate() {
        switch (_currentMovement) {
            case MovementTypeEnum.SIDEWAYS:
                MoveSideways();
                break;
            case MovementTypeEnum.IN_OUT:
                MoveInOut();
                break;
            case MovementTypeEnum.IDLE:
                // Do nothing.
                break;
        }

        // Cycle between different formation movements.
        if (Time.time - _currentMovementTime > _selectedFormationConfig.Time) {
            switch (_selectedFormationConfig.MovementType) {
                case MovementTypeEnum.SIDEWAYS:
                    ChangeCurrentMovement(MovementTypeEnum.IN_OUT);
                    break;

                case MovementTypeEnum.IN_OUT:
                    ChangeCurrentMovement(MovementTypeEnum.SIDEWAYS);
                    break;
            }
        }
    }

    public void UpdateMargins(float absoluteLeftLimit, float absoluteRightLimit) {
        _leftMargin = absoluteLeftLimit;
        _rightMargin = absoluteRightLimit;
    }

    public Vector3 FormationSlotToWorldPosition(Vector2Int formationSlot) {
        return _formationTransform.position +
            new Vector3(formationSlot.y - (_formationResource.MaxColumns - 1) / 2f,
                (_formationResource.MaxRows - 1) / 2f - formationSlot.x, 0) / _positionOffset;
    }

    private void ChangeCurrentMovement(MovementTypeEnum movementType) {
        _currentMovementTime = Time.time;
        _currentMovement = movementType;

        foreach (GameConfig.FormationMovementConfig formationConfig in _formationConfigList) {
            if (formationConfig.MovementType == movementType) {
                _selectedFormationConfig = formationConfig;
                break;
            }
        }
    }

    // Movement Patterns.

    private void MoveSideways() {
        Vector3 leftMarginWorldPos = FormationSlotToWorldPosition(new Vector2Int(0, (int)_leftMargin - 1));
        Vector3 rightMarginWorldPos = FormationSlotToWorldPosition(new Vector2Int(0, (int)_rightMargin + 1));

        Vector3 minScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 maxScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        _formationTransform.Translate(Vector3.right * _direction * _selectedFormationConfig.Speed * Time.deltaTime);

        // Change the direction if a margin was reached.
        if ((leftMarginWorldPos.x < minScreenBounds.x && _direction < 0) ||
            (rightMarginWorldPos.x > maxScreenBounds.x && _direction > 0)) {
            _direction *= -1;
        }

        // Slowly adjust back the position offset from Move IN/OUT
        if (_positionOffset < 2.55f) {
            _positionOffset += Time.deltaTime * 0.25f;
        }
    }

    private void MoveInOut() {
        _positionOffset += Time.deltaTime * _direction * _selectedFormationConfig.Speed;

        if (_positionOffset > 2.55f) {
            _positionOffset = 2.54f;
            _direction *= -1;
        }
        else if (_positionOffset < 2.0f) {
            _positionOffset = 2.01f;
            _direction *= -1;
        }
    }
}