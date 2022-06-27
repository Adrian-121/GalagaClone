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

    private List<EnemyMainController> _enemyList = new List<EnemyMainController>();
    private Dictionary<EnemyMainController, Vector2Int> _enemyToSlotCount = new Dictionary<EnemyMainController, Vector2Int>();

    public int TotalEnemies { get; private set; }
    private Dictionary<int, int> _totalEnemiesByType = new Dictionary<int, int>();
    public Dictionary<int, int> TotalEnemiesByType => _totalEnemiesByType;

    public void Construct(List<GameConfig.FormationMovementConfig> formationConfigList) {
        _movement = GetComponentInChildren<EnemyFormationMovement>();
        _movement.Construct(transform, formationConfigList);
    }
    
    public void Initialize(FormationPatternResource formationPatternResource) {
        _movement.Initialize(formationPatternResource);
        
        _formation = new FormationSlot[formationPatternResource.MaxRows][];
        _formationResource = formationPatternResource;

        TotalEnemies = 0;

        for (int i = 0; i < formationPatternResource.MaxRows; i++) {
            _formation[i] = new FormationSlot[formationPatternResource.MaxColumns];

            for (int j = 0; j < formationPatternResource.MaxColumns; j++) {
                _formation[i][j].Type = formationPatternResource.Values[i][j];
                
                _formation[i][j].IsCurrentlyUsed = false;
                _formation[i][j].IsDisabled = false;

                _formation[i][j].RowIndex = i;
                _formation[i][j].ColIndex = j;

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
        for (int i = 0; i < _formationResource.MaxRows; i++) {
            for (int j = 0; j < _formationResource.MaxColumns; j++) {
                _formation[i][j].IsCurrentlyUsed = false;
                _formation[i][j].IsDisabled = false;
            }
        }
    }

    public void OnUpdate() {
        _movement.OnUpdate();
    }

    public Vector3 GetRelativePosition(Vector2Int formationSlot) {
        return _movement.GetRelativePosition(formationSlot);
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

                _enemyList.Add(enemy);
                _enemyToSlotCount.Add(enemy, new Vector2Int(i, j));

                return new Vector2Int(i, j);
            }
        }

        return Vector2Int.zero;
    }

    /// <summary>
    /// Removes the given enemy from the formation.
    /// </summary>
    public void RemoveEnemy(EnemyMainController enemy) {
        if (!_enemyToSlotCount.ContainsKey(enemy)) { return; }

        Vector2Int slot = _enemyToSlotCount[enemy];

        _formation[slot.x][slot.y].IsCurrentlyUsed = false;
        _formation[slot.x][slot.y].IsDisabled = true;

        _enemyList.Remove(enemy);
        _enemyToSlotCount.Remove(enemy);

        ProcessLimitPositions();
    }

    private void ProcessLimitPositions() {
        int rightLimit = 0;
        int leftLimit = 0;

        // Search for the RIGHT limit of the formation.
        for (int colIndex = 0; colIndex < _formationResource.MaxColumns; colIndex++) {
            for (int rowIndex = 0; rowIndex < _formationResource.MaxRows; rowIndex++) {
                if (_formation[rowIndex][colIndex].IsCurrentlyUsed) {
                    rightLimit = colIndex;
                    break;
                }
            }
        }

        // Search for the LEFT limit of the formation.
        for (int colIndex = _formationResource.MaxColumns - 1; colIndex >= 0; colIndex--) {
            for (int rowIndex = 0; rowIndex < _formationResource.MaxRows; rowIndex++) {
                if (_formation[rowIndex][colIndex].IsCurrentlyUsed) {
                    leftLimit = colIndex;
                    break;
                }
            }
        }

        _movement.UpdateMargins(leftLimit, rightLimit);
    }
}