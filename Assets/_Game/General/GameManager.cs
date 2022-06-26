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
    private int _currentLevelSequenceID;

    private int _playerLives;

    private int _topScore;
    public int TopScore {
        get { return _topScore; }
        set {
            _topScore = value;
            _signalBus.Fire(new HighscoreInGameChangeSignal() { NewScore = _topScore });
        }
    }

    private int _currentScore;
    public int CurrentScore {
        get { return _currentScore; }
        private set {
            _currentScore = value;
            _signalBus.Fire(new CurrentScoreSignal() { Score = _currentScore });
        }
    }

    private Coroutine _gameTimeCoroutine;

    private List<LevelResource.Sequence> _activeSequences = new List<LevelResource.Sequence>();

    private bool _isGameStarted = false;

    [Inject]
    public void Construct(SignalBus signalBus, ResourceLoader resourceLoader, PlayerMainController.Factory playerFactory, PlayerSpawnPosition playerSpawnPosition) {
        _signalBus = signalBus;
        
        _signalBus.Subscribe<EnemyKilledSignal>(x => {
            CurrentScore = CurrentScore + x.Points;

            if (CurrentScore > TopScore) {
                TopScore = CurrentScore;
            }
        });

        _signalBus.Subscribe<PlayerObjectKilledSignal>(x => {
            _playerLives--;

            if (_playerLives <= 0 && _isGameStarted) {
                _signalBus.Fire(new GameOverSignal() { Highscore = _currentScore });
                StopGame();
            }

            _signalBus.Fire(new PlayerKilledSignal() { PlayerLivesLeft = _playerLives });
        });

        _resourceLoader = resourceLoader;

        _playerFactory = playerFactory;
        _playerSpawnPosition = playerSpawnPosition;

        _enemyManager = GetComponentInChildren<EnemyManager>();
        _projectileManager = GetComponentInChildren<ProjectileManager>();
    }

    public void StartGame() {
        _isGameStarted = true;
        
        if (_player == null) {
            _player = _playerFactory.Create();
        }
        
        _player.Initialize(_playerSpawnPosition.transform.position);

        _currentLevel = _resourceLoader.GetLevel($"level{_currentLevelNumber}");
        _currentLevelSequenceID = 0;
        _gameTime = 0;

        CurrentScore = 0;
        TopScore = _resourceLoader.GetHighscores().GetTop();

        _playerLives = _resourceLoader.GameConfig.PlayerLives;


        _enemyManager.Initialize();

        _gameTimeCoroutine = StartCoroutine(CountGameTime());

        _signalBus.Fire(new LevelChangedSignal() { LevelName = _currentLevel.Name });        
    }

    public void StopGame() {
        _isGameStarted = false;

        _player.Deinitialize();
        _enemyManager.Deinitialize();
    }

    private void Update() {
        if (!_isGameStarted) { return; }

        UpdateSequences();
        ProcessActiveSequences();
    }

    private void UpdateSequences() {
        if (_currentLevelSequenceID >= _currentLevel.SequenceList.Count) { return; }

        LevelResource.Sequence currentLevelSequence = _currentLevel.SequenceList[_currentLevelSequenceID];

        if (currentLevelSequence.Time < _gameTime) {
            currentLevelSequence.Reset();
            _activeSequences.Add(currentLevelSequence);
            
            _currentLevelSequenceID++;
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
                _enemyManager.SpawnEnemy((EnemyMainController.TypeEnum)sequence.EnemyType, sequence.MovementPatternName);

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