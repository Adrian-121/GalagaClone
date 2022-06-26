using UnityEngine;
using Zenject;

public class GameOverState : BaseGameState {

    [Inject] private GameManager _gameManager;
    [SerializeField] private GameObject _gameOverUI;

    public override void OnEnter() {
        _gameOverUI.SetActive(true);
        _signalBus.Subscribe<GameOverBackButtonSignal>(OnGameOverBackPressed);
    }

    public override void OnExit() {
        _gameOverUI.SetActive(false);
        _signalBus.Unsubscribe<GameOverBackButtonSignal>(OnGameOverBackPressed);
    }

    public override void OnUpdate() {
    }

    private void OnGameOverBackPressed(GameOverBackButtonSignal args) {
        if (args.Name.Length > 0) {
            HighscoreResource highscores = _resourceLoader.GetHighscores();
            highscores.AddHighscore(new HighscoreResource.Highscore() { Name = args.Name, Score = _gameManager.CurrentScore });
            _resourceLoader.SetHighscores(highscores);
        }

        _gameFSM.ChangeState(StateNameEnum.MAIN_MENU);
    }
}