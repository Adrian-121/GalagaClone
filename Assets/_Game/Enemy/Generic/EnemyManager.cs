using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyManager : MonoBehaviour, IGameControlled {

    private EnemyMainController.Factory _enemyFactory;

    private EnemyFormation _formation;

    private List<MovementPatternResource> _movementPatternList;
    private Dictionary<string, MovementPatternResource> _nameToMovementPattern = new Dictionary<string, MovementPatternResource>();

    private List<EnemyMainController> _enemyList = new List<EnemyMainController>();
    private Dictionary<int, int> _enemyTypeTotal;
    private int _totalEnemiesToDestroy;

    private Dictionary<EnemySpawnPoint.TypeEnum, Transform> _spawnPointsList = new Dictionary<EnemySpawnPoint.TypeEnum, Transform>();

    private IEnemyTarget _target;

    private ResourceLoader _resourceLoader;

    [Inject]
    public void Construct(EnemyMainController.Factory enemyFactory, ResourceLoader resourceLoader, SignalBus signalBus) {
        _enemyFactory = enemyFactory;
        _resourceLoader = resourceLoader;

        _formation = GetComponentInChildren<EnemyFormation>();
        _formation.Construct(_resourceLoader.GameConfig.FormationConfigList);

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

        signalBus.Subscribe<EnemyKilledSignal>(x => {
            _totalEnemiesToDestroy--;
            
            if (_totalEnemiesToDestroy <= 0) {
                signalBus.Fire<LevelClearedSignal>();
            }
        });
    }

    public void Initialize(IEnemyTarget target, string currentFormationName) {
        FormationPatternResource formationResource = _resourceLoader.GetFormation(currentFormationName);
        _formation.Initialize(formationResource);

        _enemyTypeTotal = new Dictionary<int, int>(_formation.TotalEnemiesByType);
        _totalEnemiesToDestroy = _formation.TotalEnemies;
        _target = target;
    }

    public void Deinitialize() {
        foreach (EnemyMainController enemy in _enemyList) {
            enemy.Deinitialize();
        }

        _formation.Deinitialize();
    }

    public void OnUpdate() {
        _formation.OnUpdate();

        foreach (EnemyMainController enemy in _enemyList) {
            enemy.OnUpdate();
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
            movementPattern.InitialRotation,
            _target);

        _enemyTypeTotal[(int)type]--;
    }
}