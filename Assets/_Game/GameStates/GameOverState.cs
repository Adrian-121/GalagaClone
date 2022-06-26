using UnityEngine;

public class GameOverState : BaseGameState {

    [SerializeField] private GameObject _gameOverUI;

    public override void OnEnter() {
        _gameOverUI.SetActive(true);
    }

    public override void OnExit() {
        _gameOverUI.SetActive(false);
    }

    public override void OnUpdate() {
    }
}