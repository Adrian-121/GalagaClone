using UnityEngine;

public class EnemyFormation : MonoBehaviour {

    [SerializeField] private float _speed = 5;    

    [SerializeField] private int _maxWidth = 8;
    [SerializeField] private int _maxHeight = 8;

    public enum FormationMovement {
        SIDEWAYS = 1,
        IN_OUT = 2,
        IDLE = 3,
    }

    public struct FormationSlot {
        public int Type;

        public bool IsCurrentlyUsed;
        public bool IsDisabled;

        public int RowIndex;
        public int ColIndex;

        public Vector3 DirectionToOrigin;
    }

    private FormationSlot[][] _formation;
    private FormationResource _formationResource;

    private float _absoluteRightLimit;
    private float _absoluteLeftLimit;

    private float _direction = -1;
    private float _directionOffset = 2.5f;
    private FormationMovement _currentMovement = FormationMovement.IN_OUT;

    public void Initialize(FormationResource formationResource) {
        _formation = new FormationSlot[formationResource.MaxRows][];
        _formationResource = formationResource;

        for (int i = 0; i < formationResource.MaxRows; i++) {
            _formation[i] = new FormationSlot[formationResource.MaxColumns];

            for (int j = 0; j < formationResource.MaxColumns; j++) {
                _formation[i][j].Type = formationResource.Values[i][j];
                
                _formation[i][j].IsCurrentlyUsed = false;
                _formation[i][j].IsDisabled = false;

                _formation[i][j].RowIndex = i;
                _formation[i][j].ColIndex = j;

                _formation[i][j].DirectionToOrigin = (Vector3.zero - GetRelativePosition(new Vector2Int(i, j))).normalized;
            }
        }
    }

    public Vector3 GetRelativePosition(Vector2Int formationSlot) {
        return transform.position +
            new Vector3(formationSlot.y - (_formationResource.MaxColumns - 1) / 2f,
                (_formationResource.MaxRows - 1)/ 2f - formationSlot.x, 0) / _directionOffset;
    }

    public Vector2Int GetFormationSlot() {
        for (int i = 0; i < _formation.Length; i++) {
            for (int j = 0; j < _formation[i].Length; j++) {
                if (!_formation[i][j].IsCurrentlyUsed && !_formation[i][j].IsDisabled && _formation[i][j].Type != 0) {
                    _formation[i][j].IsCurrentlyUsed = true;
                    ProcessLimitPositions();
                    return new Vector2Int(i, j);
                }
            }
        }

        return Vector2Int.zero;
    }

    public void ReleaseFormationSlot(Vector2Int position) {
        _formation[position.x][position.y].IsCurrentlyUsed = false;
        _formation[position.x][position.y].IsDisabled = true;
        ProcessLimitPositions();
    }

    private void ProcessLimitPositions() {
        int rightLimit = 0;
        int leftLimit = 0;

        // Going from LEFT => RIGHT and, at the first used slot, exit
        for (int colIndex = 0; colIndex < _formationResource.MaxColumns; colIndex++) {
            for (int rowIndex = 0; rowIndex < _formationResource.MaxRows; rowIndex++) {
                if (_formation[rowIndex][colIndex].IsCurrentlyUsed) {
                    rightLimit = colIndex;
                    break;
                }
            }
        }

        // Going from RIGHT => LEFT and, at the first used slot, exit
        for (int colIndex = _formationResource.MaxColumns - 1; colIndex >= 0; colIndex--) {
            for (int rowIndex = 0; rowIndex < _formationResource.MaxRows; rowIndex++) {
                if (_formation[rowIndex][colIndex].IsCurrentlyUsed) {
                    leftLimit = colIndex;
                    break;
                }
            }
        }

        _absoluteRightLimit = rightLimit;
        _absoluteLeftLimit = leftLimit;
    }

    private void Update() {
        switch (_currentMovement) {
            case FormationMovement.SIDEWAYS:
                MoveSideways();        
                break;
            case FormationMovement.IN_OUT:
                MoveInOut();
                break;
            case FormationMovement.IDLE:
                // Do nothing.
                break;
        }        
    }

    private void MoveSideways() {
        Vector3 relLeft = GetRelativePosition(new Vector2Int(0, (int)_absoluteLeftLimit - 1));
        Vector3 relRight = GetRelativePosition(new Vector2Int(0, (int)_absoluteRightLimit + 1));

        Vector3 minScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 maxScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        transform.Translate(Vector3.right * _direction * _speed * Time.deltaTime);

        if (relLeft.x < minScreenBounds.x && _direction < 0) {
            _direction *= -1;
        }
        if (relRight.x > maxScreenBounds.x && _direction > 0) {
            _direction *= -1;
        }
    }

    private void MoveInOut() {
        _directionOffset += Time.deltaTime * _direction * 0.25f;

        if (_directionOffset > 2.55f) {
            _directionOffset = 2.54f;
            _direction *= -1;
        }
        else if (_directionOffset < 1.5f) {
            _directionOffset = 1.51f;
            _direction *= -1;
        }
    }
}