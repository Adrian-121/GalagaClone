using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MainGameSystem : MonoBehaviour, IGameControlled {

    private SignalBus _signalBus;

    private EnemySystem _enemySystem;
    private ProjectileSystem _projectileSystem;
    private VFXSystem _vfxSystem;
    private List<IGameControlled> _systemsList;

    private Player _player;
    private Player.Factory _playerFactory;
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
    public void Construct(SignalBus signalBus, ResourceLoader resourceLoader, Player.Factory playerFactory, PlayerSpawnPosition playerSpawnPosition) {
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
                Deinitialize();
            }

            _signalBus.Fire(new PlayerKilledSignal() { PlayerLivesLeft = _playerLives });
        });

        _signalBus.Subscribe<LevelClearedSignal>(x => {
            _currentLevelNumber++;
            LoadLevel();
        });

        _resourceLoader = resourceLoader;

        _playerFactory = playerFactory;
        _playerSpawnPosition = playerSpawnPosition;

        _enemySystem = GetComponentInChildren<EnemySystem>();
        _projectileSystem = GetComponentInChildren<ProjectileSystem>();
        _vfxSystem = GetComponentInChildren<VFXSystem>();

        _systemsList = new List<IGameControlled>() { _enemySystem, _projectileSystem, _vfxSystem };
    }

    public void Deinitialize() {
        _isGameStarted = false;

        _player.Deinitialize();
        _activeSequences.Clear();

        foreach (IGameControlled manager in _systemsList) {
            manager.Deinitialize();
        }
    }

    public void StartNewGame() {
        _isGameStarted = true;
        
        if (_player == null) {
            _player = _playerFactory.Create();
        }
        
        _player.Initialize(_playerSpawnPosition.transform.position);
        _vfxSystem.Initialize();

        _currentLevelNumber = _resourceLoader.GameConfig.StartLevel;
        CurrentScore = 0;
        TopScore = _resourceLoader.GetHighscores().GetMaxHighscore();

        _playerLives = _resourceLoader.GameConfig.PlayerLives;
        // Update the player lives UI
        _signalBus.Fire(new PlayerKilledSignal() { PlayerLivesLeft = _playerLives });

        _gameTimeCoroutine = StartCoroutine(CountGameTime());

        LoadLevel();
    }

    private void LoadLevel() {
        _currentLevelSequenceID = 0;
        _gameTime = 0;
        _currentLevel = _resourceLoader.GetLevel($"level{_currentLevelNumber}");

        if (_currentLevel != null) {
            _enemySystem.Initialize(_player, _currentLevel.Formation);
            _signalBus.Fire(new LevelChangedSignal() { LevelName = _currentLevel.Name });
        }
        else {
            _signalBus.Fire(new GameOverSignal() { Highscore = _currentScore });
        }        
    }

    public void OnUpdate() {
        if (!_isGameStarted) { return; }

        UpdateSequences();
        ProcessActiveSequences();

        _player.OnUpdate();

        foreach (IGameControlled manager in _systemsList) {
            manager.OnUpdate();
        }
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
            if (sequence._spawned > sequence.SpawnCount) {
                sequenceScheduledForRemovalList.Add(sequence);
                continue;
            }

            if (Time.time - sequence._cooldown > sequence.Delay) {
                _enemySystem.TrySpawnEnemy((Enemy.TypeEnum)sequence.EnemyType, sequence.MovementPatternName);

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