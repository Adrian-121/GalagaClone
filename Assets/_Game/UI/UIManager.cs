using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIManager : MonoBehaviour {

    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _highscoresButton;
    [SerializeField] private Button _highscoresBackButton;
    [SerializeField] private Button _editorGameButton;

    [SerializeField] private Button _playerFireButton;

    [SerializeField] private Text _levelText;
    [SerializeField] private Text _gameTime;

    [SerializeField] private Text _highscoreText;
    [SerializeField] private Text _currentScoreText;

    [SerializeField] private List<Image> _playerLivesImageList;

    [SerializeField] private Button _muteButton;
    [SerializeField] private Image _muteButtonImage;

    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus) {
        _signalBus = signalBus;

        _signalBus.Subscribe<LevelChangedSignal>(x => StartCoroutine(DisplayLevel(x.LevelName)));
        _signalBus.Subscribe<PlayerKilledSignal>(x => UpdatePlayerLives(x.PlayerLivesLeft));
        _signalBus.Subscribe<GameTimeTickSignal>(x => _gameTime.text = x.GameTime.ToString());
        _signalBus.Subscribe<CurrentScoreSignal>(x => _currentScoreText.text = x.Score.ToString());
        _signalBus.Subscribe<SoundMuteStatusChangedSignal>(x => _muteButtonImage.fillAmount = x.IsMuted ? 1 : 0.6f);

        if (_startGameButton != null) { _startGameButton.onClick.AddListener(OnStartGamePressed); }
        if (_highscoresButton != null) { _highscoresButton.onClick.AddListener(OnHighscorePressed); }
        if (_highscoresButton != null) { _highscoresBackButton.onClick.AddListener(OnHighscoreBackPressed); }
        if (_editorGameButton != null) { _editorGameButton.onClick.AddListener(OnEditorPressed); }
        if (_playerFireButton != null) { _playerFireButton.onClick.AddListener(OnPlayerFirePressed); }
        if (_muteButton != null) { _muteButton.onClick.AddListener(OnMuteButtonPressed); };

        _levelText.gameObject.SetActive(false);
    }

    public void OnStartGamePressed() {
        _signalBus.Fire<StartGameSignal>();
        _signalBus.Fire<UIButtonPressedSignal>();
    }

    public void OnHighscorePressed() {
        _signalBus.Fire<HighscoresSignal>();
        _signalBus.Fire<UIButtonPressedSignal>();
    }

    public void OnHighscoreBackPressed() {
        _signalBus.Fire<HighscoreBackSignal>();
        _signalBus.Fire<UIButtonPressedSignal>();
    }

    public void OnEditorPressed() {
        _signalBus.Fire<EditorSignal>();
        _signalBus.Fire<UIButtonPressedSignal>();
    }

    public void OnPlayerFirePressed() {
        _signalBus.Fire<PlayerFireSignal>();
    }

    public void OnMuteButtonPressed() {
        _signalBus.Fire<SoundMuteButtonSignal>();
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