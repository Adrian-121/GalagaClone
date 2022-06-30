public class GameState : BaseGameState {

    public override void OnEnter() {
        _signalBus.Subscribe<GameOverSignal>(OnGameOver);
        
        _associatedUIWindow.SetActive(true);

        // Start a new game.
        _mainGameSystem.StartNewGame();
    }

    public override void OnExit() {
        _associatedUIWindow.SetActive(false);

        // Stop the current running game.
        _mainGameSystem.Deinitialize();

        _signalBus.Unsubscribe<GameOverSignal>(OnGameOver);
    }

    public override void OnUpdate() {
        _mainGameSystem.OnUpdate();
    }

    private void OnGameOver() {
        _gameFSM.ChangeState(StateNameEnum.GAME_OVER);
    }
}