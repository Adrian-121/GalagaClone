using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyManager : MonoBehaviour {

    private EnemyMainController.Factory _enemyFactory;

    private EnemyFormation _formation;

    private List<MovementPatternResource> _movementPatternList;
    private Dictionary<string, MovementPatternResource> _nameToMovementPattern = new Dictionary<string, MovementPatternResource>();

    private List<EnemyMainController> _enemyList = new List<EnemyMainController>();
    private Dictionary<int, int> _enemyTypeTotal;

    private Dictionary<EnemySpawnPoint.TypeEnum, Transform> _spawnPointsList = new Dictionary<EnemySpawnPoint.TypeEnum, Transform>();

    [Inject]
    public void Construct(EnemyMainController.Factory enemyFactory, ResourceLoader resourceLoader) {
        _enemyFactory = enemyFactory;

        _formation = GetComponentInChildren<EnemyFormation>();
        FormationPatternResource formationResource = resourceLoader.GetFormation("default_formation");
        _formation.Initialize(formationResource, resourceLoader.GameConfig.FormationConfigList);
        

        _movementPatternList = resourceLoader.GetMovementPatterns().Patterns;

        foreach (MovementPatternResource movementPattern in _movementPatternList) {
            _nameToMovementPattern.Add(movementPattern.Name, movementPattern);
        }

        for (int i = 0; i < Constants.ENEMY_POOL_MAX; i++) {
            EnemyMainController newEnemy = _enemyFactory.Create();
            
            newEnemy.Construct(_formation);
            newEnemy.transform.SetParent(transform);

            _enemyList.Add(newEnemy);
        }

        EnemySpawnPoint[] spawnPoints = GetComponentsInChildren<EnemySpawnPoint>();
        foreach (EnemySpawnPoint spawnPoint in spawnPoints) {
            _spawnPointsList.Add(spawnPoint.Type, spawnPoint.transform);
        }
    }

    public void Initialize() {
        _enemyTypeTotal = new Dictionary<int, int>(_formation.EnemyTypeTotal);
    }

    public void Deinitialize() {
        foreach (EnemyMainController enemy in _enemyList) {
            enemy.Deinitialize();
        }
    }

    public void SpawnEnemy(EnemyMainController.TypeEnum type, string movementPatternName) {
        EnemyMainController enemyToUse = null;

        if (_enemyTypeTotal[(int)type] <= 0) { return; }
        if (!_nameToMovementPattern.ContainsKey(movementPatternName)) { return; }

        foreach (EnemyMainController enemy in _enemyList) {
            if (!enemy.IsAlive) {
                enemyToUse = enemy;
                break;
            }
        }

        if (enemyToUse == null) { return; }

        MovementPatternResource movementPattern = _nameToMovementPattern[movementPatternName];
        enemyToUse.Initialize(type, movementPattern, 
            _spawnPointsList[(EnemySpawnPoint.TypeEnum)movementPattern.Spawner].position,
            movementPattern.InitialRotation);

        _enemyTypeTotal[(int)type]--;
    }

    
}