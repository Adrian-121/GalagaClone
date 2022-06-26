using UnityEngine;
using Zenject;

public class GameState : BaseGameState {

    [Inject] private GameManager _gameManager;

    [SerializeField] private GameObject _inGameUI;

    public override void OnEnter() {
        _inGameUI.SetActive(true);
        _gameManager.StartGame();

        _signalBus.Subscribe<GameOverSignal>(OnGameOver);
    }

    public override void OnExit() {
        _inGameUI.SetActive(false);
        _gameManager.StopGame();

        _signalBus.Unsubscribe<GameOverSignal>(OnGameOver);
    }

    public override void OnUpdate() {
    }

    private void OnGameOver() {
        _gameFSM.ChangeState(StateNameEnum.GAME_OVER);
    }
}
