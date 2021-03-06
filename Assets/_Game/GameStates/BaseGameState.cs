using UnityEngine;
using Zenject;

/// <summary>
/// Template for a Game State.
/// </summary>
public abstract class BaseGameState : MonoBehaviour {

    [SerializeField] protected GameObject _associatedUIWindow;

    [Inject] protected GameFSM _gameFSM;
    [Inject] protected readonly SignalBus _signalBus;
    [Inject] protected ResourceLoader _resourceLoader;
    [Inject] protected MainGameSystem _mainGameSystem;

    public enum StateNameEnum {
        MAIN_MENU = 1,
        GAME = 2,
        GAME_OVER = 3
    }

    [SerializeField] protected StateNameEnum _stateName;
    public StateNameEnum StateName => _stateName;

    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void OnUpdate();
}