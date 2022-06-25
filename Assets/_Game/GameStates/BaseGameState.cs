using UnityEngine;
using Zenject;

public abstract class BaseGameState : MonoBehaviour {

    [Inject] protected GameFSM _gameFSM;
    [Inject] protected readonly SignalBus _signalBus;

    public enum StateNameEnum {
        MAIN_MENU = 1,
        GAME = 2,
        HIGHSCORES = 3,
        GAME_OVER = 4
    }

    [SerializeField] protected StateNameEnum _stateName;
    public StateNameEnum StateName => _stateName;

    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void OnUpdate();
}