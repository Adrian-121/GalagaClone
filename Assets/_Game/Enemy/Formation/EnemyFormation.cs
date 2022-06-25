using System.Collections.Generic;
using UnityEngine;

public class EnemyFormation : MonoBehaviour {

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
    private Dictionary<EnemyMainController, Vector2Int> _enemyToSlot = new Dictionary<EnemyMainController, Vector2Int>();

    private Dictionary<int, int> _enemyTypeTotal = new Dictionary<int, int>();
    public Dictionary<int, int> EnemyTypeTotal => _enemyTypeTotal;
    
    public void Initialize(FormationPatternResource formationPatternResource, List<GameConfig.FormationMovementConfig> formationConfigList) {
        _movement = GetComponentInChildren<EnemyFormationMovement>();
        _movement.Construct(transform, formationPatternResource, formationConfigList);

        _formation = new FormationSlot[formationPatternResource.MaxRows][];
        _formationResource = formationPatternResource;

        for (int i = 0; i < formationPatternResource.MaxRows; i++) {
            _formation[i] = new FormationSlot[formationPatternResource.MaxColumns];

            for (int j = 0; j < formationPatternResource.MaxColumns; j++) {
                _formation[i][j].Type = formationPatternResource.Values[i][j];
                
                _formation[i][j].IsCurrentlyUsed = false;
                _formation[i][j].IsDisabled = false;

                _formation[i][j].RowIndex = i;
                _formation[i][j].ColIndex = j;

                if (!_enemyTypeTotal.ContainsKey(_formation[i][j].Type)) {
                    _enemyTypeTotal.Add(_formation[i][j].Type, 1);
                }
                else {
                    _enemyTypeTotal[_formation[i][j].Type]++;
                }
            }
        } 
    }

    public Vector3 GetRelativePosition(Vector2Int formationSlot) {
        return _movement.GetRelativePosition(formationSlot);
    }

    public Vector2Int AssignEnemy(EnemyMainController enemy) {
        for (int i = 0; i < _formation.Length; i++) {
            for (int j = 0; j < _formation[i].Length; j++) {
                if (_formation[i][j].IsCurrentlyUsed) continue;
                if (_formation[i][j].IsDisabled) continue;
                if (_formation[i][j].Type != (int)enemy.Type) continue;

                _formation[i][j].IsCurrentlyUsed = true;
                ProcessLimitPositions();

                _enemyList.Add(enemy);
                _enemyToSlot.Add(enemy, new Vector2Int(i, j));

                return new Vector2Int(i, j);
            }
        }

        return Vector2Int.zero;
    }

    public void RemoveEnemy(EnemyMainController enemy) {
        Vector2Int slot = _enemyToSlot[enemy];

        _formation[slot.x][slot.y].IsCurrentlyUsed = false;
        _formation[slot.x][slot.y].IsDisabled = true;

        _enemyList.Remove(enemy);
        _enemyToSlot.Remove(enemy);

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

        _movement.UpdateMargins(leftLimit, rightLimit);
    }

}