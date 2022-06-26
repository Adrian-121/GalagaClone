using UnityEngine;
using Zenject;

public class MainMenuState : BaseGameState {

    [SerializeField] private GameObject _mainMenuUI;

    public override void OnEnter() {
        _mainMenuUI.SetActive(true);

        _signalBus.Subscribe<StartGameSignal>(OnStartGame);
        ProcessHighScores();
    }

    public override void OnExit() {
        _mainMenuUI.SetActive(false);

        _signalBus.Unsubscribe<StartGameSignal>(OnStartGame);
    }

    public override void OnUpdate() {
    }

    private void OnStartGame() {
        _gameFSM.ChangeState(StateNameEnum.GAME);
    }

    private void ProcessHighScores() {
        string rawHighscores = PlayerPrefs.GetString(Constants.HIGHSCORES_PREFS, JsonUtility.ToJson(new HighscoreResource()));
        HighscoreResource highscores = JsonUtility.FromJson<HighscoreResource>(rawHighscores);

        _signalBus.Fire(new HighscoresProcessedSignal() { Highscores = highscores });
    }
}