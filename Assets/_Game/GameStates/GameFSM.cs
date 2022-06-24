using System.Collections.Generic;
using UnityEngine;

public class GameFSM : MonoBehaviour {

    [SerializeField] BaseGameState.StateNameEnum _entryState;

    private BaseGameState _currentState;

    private Dictionary<BaseGameState.StateNameEnum, BaseGameState> _statesMap = new Dictionary<BaseGameState.StateNameEnum, BaseGameState>();

    private void Awake() {
        BaseGameState[] gameStates = GetComponentsInChildren<BaseGameState>();

        foreach (BaseGameState gameState in gameStates) {
            _statesMap.Add(gameState.StateName, gameState);
        }

        ChangeState(_entryState);
    }

    private void Update() {
        if (_currentState == null) { return; }
        _currentState.OnUpdate();
    }

    public void ChangeState(BaseGameState.StateNameEnum newState) {
        if (_currentState != null) { 
            _currentState.OnExit();
        }

        if (_statesMap.ContainsKey(newState)) {
            _currentState = _statesMap[newState];
            _currentState.OnEnter();
        }        
    }
}