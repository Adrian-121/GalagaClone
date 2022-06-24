using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour {

    private EnemyManager _enemyManager;
    private ProjectileManager _projectileManager;

    private PlayerMainController _player;
    private PlayerMainController.Factory _playerFactory;
    private PlayerSpawnPosition _playerSpawnPosition;


    private ResourceLoader _resourceLoader;

    private int _currentLevelNumber = 1;
    private LevelResource _currentLevel;

    [Inject]
    public void Construct(ResourceLoader resourceLoader, PlayerMainController.Factory playerFactory, PlayerSpawnPosition playerSpawnPosition) {
        _resourceLoader = resourceLoader;

        _playerFactory = playerFactory;
        _playerSpawnPosition = playerSpawnPosition;

        _enemyManager = GetComponentInChildren<EnemyManager>();
        _projectileManager = GetComponentInChildren<ProjectileManager>();
    }

    public void StartGame() {
        _player = _playerFactory.Create();
        _player.transform.position = _playerSpawnPosition.transform.position;

        _enemyManager.StartGame();

        _currentLevel = _resourceLoader.GetLevel($"level{_currentLevelNumber}");

        Debug.Log("qqq");
    }

    public void CleanupGame() {

    }
}