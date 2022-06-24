using UnityEngine;
using Zenject;

public class GameState : BaseGameState {

    [Inject]
    private GameManager _gameManager;

    [SerializeField] private GameObject _inGameUI;

    public override void OnEnter() {
        _inGameUI.SetActive(true);
        _gameManager.StartGame();
    }

    public override void OnExit() {
        _inGameUI.SetActive(false);
        _gameManager.CleanupGame();
    }

    public override void OnUpdate() {
    }
}
