using UnityEngine;
using Zenject;

public class LevelEditor : MonoBehaviour {

    public static LevelEditor Instance;

    [SerializeField] private GameObject _patternsWindow;
    [SerializeField] private GameObject _levelsWindow;

    [Inject]
    private ResourceLoader _resourceLoader;
    private MovementPatternResource.MovementPatternContainer _allPatterns;

    private PatternsEditor _patternsEditor;

    private void Awake() {
        Instance = this;

        _allPatterns = _resourceLoader.GetMovementPatterns();

        _patternsEditor = GetComponentInChildren<PatternsEditor>();
    }

    public void OnPatternsButtonPressed() {
        _patternsWindow.SetActive(!_patternsWindow.activeInHierarchy);

        if (_patternsWindow.activeInHierarchy) {
            _levelsWindow.SetActive(false);
            _patternsEditor.RefreshPatternList(_allPatterns);
        }
    }

    public void OnLevelsWindowPressed() {
        _levelsWindow.SetActive(!_levelsWindow.activeInHierarchy);
        if (_levelsWindow.activeInHierarchy) {
            _patternsWindow.SetActive(false);
        }
    }

    public void OnNewPattern() {
        _allPatterns.Patterns.Add(new MovementPatternResource() { Name = "p" + Random.Range(0, 5000) });
        _patternsEditor.RefreshPatternList(_allPatterns);
    }
}