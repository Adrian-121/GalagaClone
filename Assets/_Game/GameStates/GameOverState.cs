public class GameOverState : BaseGameState {

    public override void OnEnter() {
        _associatedUIWindow.SetActive(true);
        _signalBus.Subscribe<GameOverBackButtonSignal>(OnGameOverBackPressed);
    }

    public override void OnExit() {
        _associatedUIWindow.SetActive(false);
        _signalBus.Unsubscribe<GameOverBackButtonSignal>(OnGameOverBackPressed);
    }

    public override void OnUpdate() {
    }

    private void OnGameOverBackPressed(GameOverBackButtonSignal args) {
        // Add a new highscore, if the name's length is greater than 0.
        if (args.Name.Length > 0) {
            HighscoreResource highscores = _resourceLoader.GetHighscores();
            highscores.AddHighscore(new HighscoreResource.Highscore() { Name = args.Name, Score = _gameManager.CurrentScore });
            _resourceLoader.SetHighscores(highscores);
        }

        // Go back to the main menu.
        _gameFSM.ChangeState(StateNameEnum.MAIN_MENU);
    }
}