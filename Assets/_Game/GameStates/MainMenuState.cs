public class MainMenuState : BaseGameState {

    public override void OnEnter() {
        _associatedUIWindow.SetActive(true);

        _signalBus.Subscribe<StartGameSignal>(OnStartGame);

        ProcessHighScores();
    }

    public override void OnExit() {
        _associatedUIWindow.SetActive(false);

        _signalBus.Unsubscribe<StartGameSignal>(OnStartGame);
    }

    public override void OnUpdate() {
    }

    private void OnStartGame() {
        _gameFSM.ChangeState(StateNameEnum.GAME);
    }

    private void ProcessHighScores() {
        HighscoreResource highscores = _resourceLoader.GetHighscores();
        _signalBus.Fire(new HighscoresProcessedSignal() { Highscores = highscores });
    }
}