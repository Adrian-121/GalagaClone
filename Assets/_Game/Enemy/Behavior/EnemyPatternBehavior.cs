using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Moves the enemy in the specified pattern.
/// </summary>
public class EnemyPatternBehavior : BaseEnemyBehavior {

    public UnityEvent OnPatternFinished;

    private MovementPatternResource _movementPattern;
    private int _currentSequenceID;
    private float _currentTimer;

    private bool _isFinished;
    
    public void Construct(Enemy enemy) {
        _enemy = enemy;
    }

    public void Initialize(MovementPatternResource movementPattern, GameConfig.EnemyConfig config) {
        _movementPattern = movementPattern;
        _config = config;
        
        _currentSequenceID = 0;
        _currentTimer = Time.time;

        _isFinished = false;
    }

    public override void OnEnter() {        
    }

    public override void OnExit() {
    }

    public override void OnUpdate() {
        if (_isFinished) { return; }

        ApplyMovement();
        ProcessCurrentSequence();                
    }

    /// <summary>
    /// Checks the current sequence and pattern's states.
    /// </summary>
    private void ProcessCurrentSequence() {
        // Is the current sequence finished?
        if (Time.time - _currentTimer > _movementPattern.SequenceList[_currentSequenceID].TimeInSeconds) {
            _currentSequenceID++;
            _currentTimer = Time.time;
        }

        // Are all the sequences finished? => pattern finished.
        if (_currentSequenceID >= _movementPattern.SequenceList.Count) {
            _isFinished = true;
            OnPatternFinished.Invoke();
        }
    }

    /// <summary>
    /// Applies the movement from the current sequence to the enemy.
    /// </summary>
    private void ApplyMovement() {
        if (_isFinished) { return; }

        float speedMultiplier = _movementPattern.SequenceList[_currentSequenceID].SpeedMultiplier;
        float angle = _movementPattern.SequenceList[_currentSequenceID].AngleInDegrees;

        _enemy.transform.Translate(Vector3.up * (_config.speed * speedMultiplier) * Time.deltaTime);
        _enemy.transform.Rotate(Vector3.forward, angle * Time.deltaTime);
    }
}