using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour {

    private SignalBus _signalBus;

    private EnemyManager _enemyManager;
    private ProjectileManager _projectileManager;

    private PlayerMainController _player;
    private PlayerMainController.Factory _playerFactory;
    private PlayerSpawnPosition _playerSpawnPosition;

    private ResourceLoader _resourceLoader;

    private int _currentLevelNumber = 1;
    private LevelResource _currentLevel;
    private int _gameTime;
    private int _lastLevelSequenceID;
    private LevelResource.Sequence LastLevelSequence => _currentLevel.SequenceList[_lastLevelSequenceID];

    private Coroutine _gameTimeCoroutine;

    private List<LevelResource.Sequence> _activeSequences = new List<LevelResource.Sequence>();

    private bool _isGameStarted = false;

    [Inject]
    public void Construct(SignalBus signalBus, ResourceLoader resourceLoader, PlayerMainController.Factory playerFactory, PlayerSpawnPosition playerSpawnPosition) {
        _signalBus = signalBus;
        _resourceLoader = resourceLoader;

        _playerFactory = playerFactory;
        _playerSpawnPosition = playerSpawnPosition;

        _enemyManager = GetComponentInChildren<EnemyManager>();
        _projectileManager = GetComponentInChildren<ProjectileManager>();
    }

    public void StartGame() {
        _isGameStarted = true;

        _player = _playerFactory.Create();
        _player.transform.position = _playerSpawnPosition.transform.position;


        _currentLevel = _resourceLoader.GetLevel($"level{_currentLevelNumber}");
        _lastLevelSequenceID = 0;
        

        _gameTimeCoroutine = StartCoroutine(CountGameTime());

        _signalBus.Fire(new LevelChangedSignal() { LevelName = _currentLevel.Name });
    }

    public void StopGame() {
        _isGameStarted = false;

    }

    private void Update() {
        if (!_isGameStarted) { return; }

        UpdateSequences();
        ProcessActiveSequences();
    }

    private void UpdateSequences() {
        if (LastLevelSequence.Time > _gameTime) {
            LastLevelSequence.Reset();
            _activeSequences.Add(LastLevelSequence);
            _lastLevelSequenceID++;
        }
    }

    private void ProcessActiveSequences() {
        List<LevelResource.Sequence> sequenceScheduledForRemovalList = new List<LevelResource.Sequence>();

        foreach (LevelResource.Sequence sequence in _activeSequences) {
            if (sequence._spawned >= sequence.SpawnCount) {
                sequenceScheduledForRemovalList.Add(sequence);
                continue;
            }

            if (Time.time - sequence._cooldown > sequence.Delay) {
                // Spawn here
                sequence._cooldown = Time.time;
                sequence._spawned++;
            }
        }

        foreach (LevelResource.Sequence sequence in sequenceScheduledForRemovalList) {
            _activeSequences.Remove(sequence);
        }
    }

    private IEnumerator CountGameTime() {
        _gameTime = 0;

        while (_isGameStarted) {
            _gameTime += 1;
            _signalBus.Fire(new GameTimeTickSignal() { GameTime = _gameTime });
            yield return new WaitForSeconds(1);
        }

        _gameTimeCoroutine = null;
        yield return null;
    }
}