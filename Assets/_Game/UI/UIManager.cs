using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIManager : MonoBehaviour {

    [SerializeField] private Button _startGameButton;

    [SerializeField] private Button _playerFireButton;

    [SerializeField] private Text _levelText;
    [SerializeField] private Text _gameTime;

    [SerializeField] private Text _highscoreText;
    [SerializeField] private Text _currentScoreText;

    [SerializeField] private Text _hallOfFameText;

    [SerializeField] private List<Image> _playerLivesImageList;

    [SerializeField] private Button _muteButton;
    [SerializeField] private Image _muteButtonImage;

    [SerializeField] private InputField _highscoreNameInput;
    [SerializeField] private Button _gameOverBackButton;

    [SerializeField] private Text _fpsText;

    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus) {
        _signalBus = signalBus;

        _signalBus.Subscribe<LevelChangedSignal>(x => StartCoroutine(DisplayLevel(x.LevelName)));
        _signalBus.Subscribe<PlayerKilledSignal>(x => UpdatePlayerLives(x.PlayerLivesLeft));
        _signalBus.Subscribe<GameTimeTickSignal>(x => _gameTime.text = x.GameTime.ToString());
        _signalBus.Subscribe<CurrentScoreSignal>(x => _currentScoreText.text = x.Score.ToString());
        _signalBus.Subscribe<SoundMuteStatusChangedSignal>(x => _muteButtonImage.fillAmount = x.IsMuted ? 1 : 0.6f);
        _signalBus.Subscribe<HighscoresProcessedSignal>(x => OnHighscoresReceived(x.Highscores));
        _signalBus.Subscribe<HighscoreInGameChangeSignal>(x => _highscoreText.text = x.NewScore.ToString());

        if (_startGameButton != null) { _startGameButton.onClick.AddListener(OnStartGamePressed); }
        if (_playerFireButton != null) { _playerFireButton.onClick.AddListener(OnPlayerFirePressed); }
        if (_muteButton != null) { _muteButton.onClick.AddListener(OnMuteButtonPressed); };
        if (_gameOverBackButton != null) { _gameOverBackButton.onClick.AddListener(OnGameOverBackButtonPressed); }

        _levelText.gameObject.SetActive(false);
    }

    private void Update() {
        _fpsText.text = ((int)(1f / Time.unscaledDeltaTime)).ToString();
    }

    public void OnStartGamePressed() {
        _signalBus.Fire<StartGameSignal>();
        _signalBus.Fire<UIButtonPressedSignal>();
    }
  
    public void OnPlayerFirePressed() {
        _signalBus.Fire<PlayerFireSignal>();
    }

    public void OnMuteButtonPressed() {
        _signalBus.Fire<SoundMuteButtonSignal>();
    }

    public void OnGameOverBackButtonPressed() {
        _signalBus.Fire(new GameOverBackButtonSignal() { Name = _highscoreNameInput.text });
        _highscoreNameInput.text = string.Empty;
    }

    public void OnHighscoresReceived(HighscoreResource highscores) {
        StringBuilder hallOfFame = new StringBuilder();

        List<HighscoreResource.Highscore> highscoreList = highscores.GetHighscoreList();

        for (int i = 0; i < highscoreList.Count; i++) {
            HighscoreResource.Highscore highscore = highscoreList[i];
            hallOfFame.Append("[");
            hallOfFame.Append(i);
            hallOfFame.Append("] - ");
            hallOfFame.Append(highscore.Name);
            hallOfFame.Append(" / ");
            hallOfFame.Append(highscore.Score);
            hallOfFame.Append("\n");
        }

        _hallOfFameText.text = hallOfFame.ToString();
    }

    private void UpdatePlayerLives(int playerLivesLeft) {
        for (int i = 0; i < _playerLivesImageList.Count; i++) {
            _playerLivesImageList[i].gameObject.SetActive(i < playerLivesLeft);
        }
    }

    private IEnumerator DisplayLevel(string levelName) {
        _levelText.text = levelName;
        _levelText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        _levelText.gameObject.SetActive(false);
    }
}