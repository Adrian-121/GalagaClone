using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemySystem : MonoBehaviour, IGameControlled {

    private Enemy.Factory _enemyFactory;
    private ResourceLoader _resourceLoader;

    private EnemyFormation _formation;

    private List<MovementPatternResource> _movementPatternList;
    private Dictionary<string, MovementPatternResource> _nameToMovementPattern = new Dictionary<string, MovementPatternResource>();

    private List<Enemy> _enemyList = new List<Enemy>();
    private Dictionary<int, int> _enemyTypeTotal;
    private int _totalEnemiesToDestroy;

    private Dictionary<EnemySpawnPoint.TypeEnum, Transform> _spawnPointsList = new Dictionary<EnemySpawnPoint.TypeEnum, Transform>();

    private IEnemyTarget _target;

    [Inject]
    public void Construct(Enemy.Factory enemyFactory, ResourceLoader resourceLoader, SignalBus signalBus) {
        _enemyFactory = enemyFactory;
        _resourceLoader = resourceLoader;

        _formation = GetComponentInChildren<EnemyFormation>();
        _formation.Construct(_resourceLoader.GameConfig.FormationConfigList);

        LoadMovementPatterns(resourceLoader);
        PopulateEnemyPool();
        InitializeSpawnPoints();

        // Subscribe to events.

        signalBus.Subscribe<EnemyKilledSignal>(x => {
            _totalEnemiesToDestroy--;
            
            if (_totalEnemiesToDestroy <= 0) {
                signalBus.Fire<LevelClearedSignal>();
            }
        });
    }

    private void LoadMovementPatterns(ResourceLoader resourceLoader) {
        _movementPatternList = resourceLoader.GetMovementPatterns().Patterns;
        foreach (MovementPatternResource movementPattern in _movementPatternList) {
            _nameToMovementPattern.Add(movementPattern.Name, movementPattern);
        }
    }

    private void PopulateEnemyPool() {
        for (int i = 0; i < Constants.ENEMY_POOL_MAX; i++) {
            Enemy newEnemy = _enemyFactory.Create();

            newEnemy.Construct(_formation);
            newEnemy.transform.SetParent(transform);

            _enemyList.Add(newEnemy);
        }
    }

    private void InitializeSpawnPoints() {
        EnemySpawnPoint[] spawnPoints = GetComponentsInChildren<EnemySpawnPoint>();
        foreach (EnemySpawnPoint spawnPoint in spawnPoints) {
            _spawnPointsList.Add(spawnPoint.Type, spawnPoint.transform);
        }
    }

    public void Initialize(IEnemyTarget target, string currentFormationName) {
        FormationPatternResource formationResource = _resourceLoader.GetFormation(currentFormationName);
        _formation.Initialize(formationResource);

        _enemyTypeTotal = new Dictionary<int, int>(_formation.TotalEnemiesByType);
        _totalEnemiesToDestroy = _formation.TotalEnemies;
        _target = target;
    }

    public void Deinitialize() {
        foreach (Enemy enemy in _enemyList) {
            enemy.Deinitialize();
        }

        _formation.Deinitialize();
    }

    public void OnUpdate() {
        _formation.OnUpdate();

        foreach (Enemy enemy in _enemyList) {
            enemy.OnUpdate();
        }
    }

    /// <summary>
    /// Tries to spawn a new enemy, if the formation is not filled yet.
    /// </summary>
    public void TrySpawnEnemy(Enemy.TypeEnum type, string movementPatternName) {
        Enemy enemyToUse = null;

        // Check if formation filled.
        if (_enemyTypeTotal[(int)type] <= 0) { return; }

        // Check if there a movement pattern with the corresponding name.
        if (!_nameToMovementPattern.ContainsKey(movementPatternName)) { return; }
        MovementPatternResource movementPattern = _nameToMovementPattern[movementPatternName];

        // Get a fresh enemy from the pool.
        foreach (Enemy enemy in _enemyList) {
            if (!enemy.IsAlive) {
                enemyToUse = enemy;
                break;
            }
        }

        if (enemyToUse == null) { return; }

        // Initialize the enemy.
        enemyToUse.Initialize(type, movementPattern, 
            _spawnPointsList[(EnemySpawnPoint.TypeEnum)movementPattern.Spawner].position,
            movementPattern.InitialRotation,
            _target);

        // Counting down the remaining enemies for this type.
        _enemyTypeTotal[(int)type]--;
    }
}