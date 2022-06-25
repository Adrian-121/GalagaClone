using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyManager : MonoBehaviour {

    private EnemyMainController.Factory _enemyFactory;

    private EnemyFormation _formation;

    private List<MovementPatternResource> _movementPatternList;
    private Dictionary<string, MovementPatternResource> _nameToMovementPattern = new Dictionary<string, MovementPatternResource>();

    private List<EnemyMainController> _enemyList;

    [Inject]
    public void Construct(EnemyMainController.Factory enemyFactory, ResourceLoader resourceLoader) {
        _enemyFactory = enemyFactory;

        _formation = GetComponentInChildren<EnemyFormation>();
        FormationResource formationResource = resourceLoader.GetFormation("default_formation");
        _formation.Initialize(formationResource);

        _movementPatternList = resourceLoader.GetMovementPatterns();
        foreach (MovementPatternResource movementPattern in _movementPatternList) {
            _nameToMovementPattern.Add(movementPattern.Name, movementPattern);
        }

        for (int i = 0; i < Constants.ENEMY_POOL_MAX; i++) {
            EnemyMainController newEnemy = _enemyFactory.Create();
            newEnemy.Construct(_formation);
            _enemyList.Add(newEnemy);
        }
    }
    public void SpawnEnemy(EnemyMainController.TypeEnum type, string movementPatternName) {
        EnemyMainController enemyToUse = null;

        foreach (EnemyMainController enemy in _enemyList) {
            if (!enemy.IsAlive) {
                enemyToUse = enemy;
                break;
            }
        }

        if (enemyToUse == null) { return; }

        enemyToUse.Initialize(type, _nameToMovementPattern[_nameToMovementPattern]);
    }
}