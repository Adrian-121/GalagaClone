using System.Collections;
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

    private float _direction = -1;
    private float _directionOffset = 2.5f;

    private FormationPatternResource _formationResource;
    private List<GameConfig.FormationMovementConfig> _formationConfigList;

    private GameConfig.FormationMovementConfig selectedFormationConfig;

    private float _absoluteRightLimit;
    private float _absoluteLeftLimit;

    private Transform _formationTransform;

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

        if (Time.time - _currentMovementTime > selectedFormationConfig.Time) {
            switch (selectedFormationConfig.MovementType) {
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
        _absoluteLeftLimit = absoluteLeftLimit;
        _absoluteRightLimit = absoluteRightLimit;
    }

    private void ChangeCurrentMovement(MovementTypeEnum movementType) {
        _currentMovementTime = Time.time;
        _currentMovement = movementType;

        foreach (GameConfig.FormationMovementConfig formationConfig in _formationConfigList) {
            if (formationConfig.MovementType == movementType) {
                selectedFormationConfig = formationConfig;
                break;
            }
        }
    }

    private void MoveSideways() {
        Vector3 relLeft = GetRelativePosition(new Vector2Int(0, (int)_absoluteLeftLimit - 1));
        Vector3 relRight = GetRelativePosition(new Vector2Int(0, (int)_absoluteRightLimit + 1));

        Vector3 minScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 maxScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        _formationTransform.Translate(Vector3.right * _direction * selectedFormationConfig.Speed * Time.deltaTime);

        if (relLeft.x < minScreenBounds.x && _direction < 0) {
            _direction *= -1;
        }
        if (relRight.x > maxScreenBounds.x && _direction > 0) {
            _direction *= -1;
        }

        if (_directionOffset < 2.55f) {
            _directionOffset += Time.deltaTime * 0.25f;
        }
    }

    private void MoveInOut() {
        _directionOffset += Time.deltaTime * _direction * selectedFormationConfig.Speed;

        if (_directionOffset > 2.55f) {
            _directionOffset = 2.54f;
            _direction *= -1;
        }
        else if (_directionOffset < 2.0f) {
            _directionOffset = 2.01f;
            _direction *= -1;
        }
    }

    public Vector3 GetRelativePosition(Vector2Int formationSlot) {
        return _formationTransform.position +
            new Vector3(formationSlot.y - (_formationResource.MaxColumns - 1) / 2f,
                (_formationResource.MaxRows - 1) / 2f - formationSlot.x, 0) / _directionOffset;
    }

}