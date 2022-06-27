using System.Collections.Generic;
using UnityEngine;

public class EnemyFormation : MonoBehaviour, IGameControlled {

    public struct FormationSlot {
        public int Type;

        public bool IsCurrentlyUsed;
        public bool IsDisabled;

        public int RowIndex;
        public int ColIndex;
    }

    private FormationSlot[][] _formation;
    private FormationPatternResource _formationResource;

    private EnemyFormationMovement _movement;

    public int TotalEnemies { get; private set; }
    private Dictionary<int, int> _totalEnemiesByType = new Dictionary<int, int>();
    public Dictionary<int, int> TotalEnemiesByType => _totalEnemiesByType;

    public void Construct(List<GameConfig.FormationMovementConfig> formationConfigList) {
        _movement = GetComponentInChildren<EnemyFormationMovement>();
        _movement.Construct(transform, formationConfigList);
    }
    
    public void Initialize(FormationPatternResource formationPatternResource) {
        _movement.Initialize(formationPatternResource);
        _formationResource = formationPatternResource;

        // Clear the number of possible enemies.
        TotalEnemies = 0;
        _totalEnemiesByType.Clear();

        // Create a new formation.
        _formation = new FormationSlot[formationPatternResource.MaxRows][];

        for (int i = 0; i < formationPatternResource.MaxRows; i++) {
            _formation[i] = new FormationSlot[formationPatternResource.MaxColumns];

            for (int j = 0; j < formationPatternResource.MaxColumns; j++) {
                _formation[i][j].Type = formationPatternResource.Values[i][j];
                
                _formation[i][j].IsCurrentlyUsed = false;
                _formation[i][j].IsDisabled = false;

                _formation[i][j].RowIndex = i;
                _formation[i][j].ColIndex = j;

                // Count the enemies supported by this formation.

                if (!_totalEnemiesByType.ContainsKey(_formation[i][j].Type)) {
                    _totalEnemiesByType.Add(_formation[i][j].Type, 1);
                }
                else {
                    _totalEnemiesByType[_formation[i][j].Type]++;
                }

                if (_formation[i][j].Type != 0) {
                    TotalEnemies++;
                }                
            }
        } 
    }

    public void Deinitialize() {
    }

    public void OnUpdate() {
        _movement.OnUpdate();
    }

    public Vector3 FormationSlotToWorldPosition(Vector2Int formationSlot) {
        return _movement.FormationSlotToWorldPosition(formationSlot);
    }

    /// <summary>
    /// Assigns an enemy to the formation and returns the formation position for it.
    /// </summary>
    public Vector2Int AssignEnemy(EnemyMainController enemy) {
        for (int i = 0; i < _formation.Length; i++) {
            for (int j = 0; j < _formation[i].Length; j++) {
                if (_formation[i][j].IsCurrentlyUsed) continue;
                if (_formation[i][j].IsDisabled) continue;
                if (_formation[i][j].Type != (int)enemy.Type) continue;

                _formation[i][j].IsCurrentlyUsed = true;
                ProcessLimitPositions();

                return new Vector2Int(i, j);
            }
        }

        return Vector2Int.zero;
    }

    /// <summary>
    /// Removes the given enemy from the formation.
    /// </summary>
    public void RemoveEnemy(Vector2Int formationSlot) {
        // Mark this slot to not be used by another enemy in this run.
        _formation[formationSlot.x][formationSlot.y].IsCurrentlyUsed = false;
        _formation[formationSlot.x][formationSlot.y].IsDisabled = true;

        ProcessLimitPositions();
    }

    /// <summary>
    /// Calculates the maximum left/right limits of the current formation.
    /// These change based on enemy assignment/removal from the formation.
    /// </summary>
    private void ProcessLimitPositions() {
        int rightLimit = 0;
        int leftLimit = 0;

        // Search for the RIGHT limit of the formation.
        for (int colIndex = 0; colIndex < _formationResource.MaxColumns; colIndex++) {
            for (int rowIndex = 0; rowIndex < _formationResource.MaxRows; rowIndex++) {
                if (_formation[rowIndex][colIndex].IsCurrentlyUsed) {
                    // First found, exit.
                    rightLimit = colIndex;
                    break;
                }
            }
        }

        // Search for the LEFT limit of the formation.
        for (int colIndex = _formationResource.MaxColumns - 1; colIndex >= 0; colIndex--) {
            for (int rowIndex = 0; rowIndex < _formationResource.MaxRows; rowIndex++) {
                if (_formation[rowIndex][colIndex].IsCurrentlyUsed) {
                    // First found, exit.
                    leftLimit = colIndex;
                    break;
                }
            }
        }

        _movement.UpdateMargins(leftLimit, rightLimit);
    }
}