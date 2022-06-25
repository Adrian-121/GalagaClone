using UnityEngine;

public class MainMenuState : BaseGameState {

    [SerializeField] private GameObject _mainMenuUI;

    public override void OnEnter() {
        _mainMenuUI.SetActive(true);

        _signalBus.Subscribe<StartGameSignal>(OnStartGame);
        _signalBus.Subscribe<HighscoresSignal>(OnHighscores);
        _signalBus.Subscribe<EditorSignal>(OnEditor);
    }

    public override void OnExit() {
        _mainMenuUI.SetActive(false);

        _signalBus.Unsubscribe<StartGameSignal>(OnStartGame);
        _signalBus.Unsubscribe<HighscoresSignal>(OnHighscores);
        _signalBus.Unsubscribe<EditorSignal>(OnEditor);
    }

    public override void OnUpdate() {
    }

    private void OnStartGame() {
        _gameFSM.ChangeState(StateNameEnum.GAME);
    }

    private void OnHighscores() {
        _gameFSM.ChangeState(StateNameEnum.HIGHSCORES);
    }

    private void OnEditor() {

    }
}