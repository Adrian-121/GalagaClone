using UnityEngine;
using Zenject;

public class HighscoreState : BaseGameState {

    [SerializeField] private GameObject _highScoreUI;

    public override void OnEnter() {
        _highScoreUI.SetActive(true);

        _signalBus.Subscribe<HighscoreBackSignal>(OnHighscoreBack);
    }

    public override void OnExit() {
        _highScoreUI.SetActive(false);

        _signalBus.Unsubscribe<HighscoreBackSignal>(OnHighscoreBack);
    }

    public override void OnUpdate() {
    }

    private void OnHighscoreBack() {
        _gameFSM.ChangeState(StateNameEnum.MAIN_MENU);    
    }
}