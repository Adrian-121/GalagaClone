using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIManager : MonoBehaviour {

    [Inject]
    private readonly SignalBus _signalBus;

    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _highscoresButton;
    [SerializeField] private Button _highscoresBackButton;
    [SerializeField] private Button _editorGameButton;

    [SerializeField] private Button _playerFireButton;

    private void Awake() {
        if (_startGameButton != null) { _startGameButton.onClick.AddListener(OnStartGamePressed); }
        if (_highscoresButton != null) { _highscoresButton.onClick.AddListener(OnHighscorePressed); }
        if (_highscoresButton != null) { _highscoresBackButton.onClick.AddListener(OnHighscoreBackPressed); }
        if (_editorGameButton != null) { _editorGameButton.onClick.AddListener(OnEditorPressed); }
        if (_playerFireButton != null) { _playerFireButton.onClick.AddListener(OnPlayerFirePressed); }
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
}