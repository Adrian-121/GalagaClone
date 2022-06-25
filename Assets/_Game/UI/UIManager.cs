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

    [SerializeField] private List<Image> _playerLivesImageList;

    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus) {
        _signalBus = signalBus;

        _signalBus.Subscribe<LevelChangedSignal>(x => StartCoroutine(DisplayLevel(x.LevelName)));
        _signalBus.Subscribe<PlayerKilledSignal>(x => UpdatePlayerLives(x.PlayerLivesLeft));
        _signalBus.Subscribe<GameTimeTickSignal>(x => _gameTime.text = x.GameTime.ToString());

        if (_startGameButton != null) { _startGameButton.onClick.AddListener(OnStartGamePressed); }
        if (_highscoresButton != null) { _highscoresButton.onClick.AddListener(OnHighscorePressed); }
        if (_highscoresButton != null) { _highscoresBackButton.onClick.AddListener(OnHighscoreBackPressed); }
        if (_editorGameButton != null) { _editorGameButton.onClick.AddListener(OnEditorPressed); }
        if (_playerFireButton != null) { _playerFireButton.onClick.AddListener(OnPlayerFirePressed); }

        _levelText.gameObject.SetActive(false);
    }

    public void OnStartGamePressed() {
        _signalBus.Fire<StartGameSignal>();
    }

    public void OnHighscorePressed() {
        _signalBus.Fire<HighscoresSignal>();
    }

    public void OnHighscoreBackPressed() {
        _signalBus.Fire<HighscoreBackSignal>();
    }

    public void OnEditorPressed() {
        _signalBus.Fire<EditorSignal>();
    }

    public void OnPlayerFirePressed() {
        _signalBus.Fire<PlayerFireSignal>();
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