using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PatternsEditor : MonoBehaviour {

    [Inject] private EnemyMainController.Factory _enemyFactory;
    private EnemyMainController _currentEnemy;

    [SerializeField] private GameObject _patternsWindow;
    [SerializeField] private GameObject _patternEditWindow;

    [SerializeField] private InputField _patternName;
    [SerializeField] private Dropdown _patternStart;
    [SerializeField] private InputField _patternInitialRotation;

    [SerializeField] private PatternEntry _patternEntryPrefab;
    [SerializeField] private Transform _patternEntryParent;
    private List<PatternEntry> _patternEntryList = new List<PatternEntry>();

    [SerializeField] private PatternMenuItem _patternMenuItemPrefab;
    [SerializeField] private Transform _patternMenuItemParent;
    private List<PatternMenuItem> _patternMenuItemList = new List<PatternMenuItem>();

    private MovementPatternResource _currentPatternResource;
    private MovementPatternResource.MovementPatternContainer _allPatterns;

    [SerializeField] private List<EnemySpawnPoint> _spawnPoints;

    // Pattern Menu List
    public void RefreshPatternList(MovementPatternResource.MovementPatternContainer allPatterns) {
        _allPatterns = allPatterns;

        for (int i = 0; i < _patternMenuItemList.Count; i++) {
            _patternMenuItemList[i].OnEdit.RemoveAllListeners();
            Destroy(_patternMenuItemList[i].gameObject);
        }

        _patternMenuItemList.Clear();

        foreach (MovementPatternResource patternResource in allPatterns.Patterns) {
            PatternMenuItem newPatternMenuItem = Instantiate(_patternMenuItemPrefab);
            newPatternMenuItem.transform.SetParent(_patternMenuItemParent);
            newPatternMenuItem.Initialize(patternResource);
            newPatternMenuItem.OnEdit.AddListener(OnPatternMenuItemEdit);

            _patternMenuItemList.Add(newPatternMenuItem);
        }
    }

    public void OnPatternMenuItemEdit(MovementPatternResource patternResource) {
        _patternsWindow.SetActive(false);
        _patternEditWindow.SetActive(true);

        LoadPatternResource(patternResource);
    }

    public void OnNewPattern() {

    }

    // Pattern Resource Editing

    public void LoadPatternResource(MovementPatternResource newPatternResource) {
        _currentPatternResource = newPatternResource;
        _patternName.text = _currentPatternResource.Name;
        _patternStart.value = _currentPatternResource.Spawner;
        _patternInitialRotation.text = _currentPatternResource.InitialRotation.ToString();

        ClearPatternEntryList();
        foreach (MovementPatternResource.Sequence sequence in newPatternResource.SequenceList) {
            CreateNewPatternEntry(sequence);
        }
    }

    public void OnTestPatternPressed() {
        if (_currentEnemy != null) {
            Destroy(_currentEnemy.gameObject);
        }

        _currentPatternResource.Name = _patternName.text;
        _currentPatternResource.Spawner = _patternStart.value;
        _currentPatternResource.InitialRotation = float.Parse(_patternInitialRotation.text);

        _currentEnemy = _enemyFactory.Create();
        _currentEnemy.Construct(null);
        _currentEnemy.Initialize(EnemyMainController.TypeEnum.GRUNT, _currentPatternResource, 
            GetSpawnPointPosition((EnemySpawnPoint.TypeEnum)_currentPatternResource.Spawner),
            _currentPatternResource.InitialRotation,
            null);
    }

    public void OnSavePatternPressed() {
        _currentPatternResource.Name = _patternName.text;
        _currentPatternResource.Spawner = _patternStart.value;
        _currentPatternResource.InitialRotation = float.Parse(_patternInitialRotation.text);

        File.WriteAllText("D:/Projects/Interviews/Superplay/GalagaClone/Assets/_Game/Resources/" + Constants.MOVEMENT_PATTERNS + ".json", JsonUtility.ToJson(_allPatterns).ToString());
    }

    public void OnDeletePatternPressed() {

    }

    public void OnNewPatternEntryPressed() {
        CreateNewPatternEntry(null);
    }

    private void ClearPatternEntryList() {
        for (int i = 0; i < _patternEntryList.Count; i++) {
            _patternEntryList[i].OnDelete.RemoveAllListeners();
            Destroy(_patternEntryList[i].gameObject);
        }

        _patternEntryList.Clear();
    }

    private void CreateNewPatternEntry(MovementPatternResource.Sequence sequence) {
        if (sequence == null) {
            sequence = new MovementPatternResource.Sequence();
            _currentPatternResource.SequenceList.Add(sequence);
        }

        PatternEntry newPatternEntry = Instantiate(_patternEntryPrefab);
        newPatternEntry.transform.SetParent(_patternEntryParent);
        newPatternEntry.transform.SetSiblingIndex(1);
        newPatternEntry.Initialize(sequence);
        newPatternEntry.OnDelete.AddListener(OnDeletePatternEntry);

        _patternEntryList.Add(newPatternEntry);
    }

    private void OnDeletePatternEntry(PatternEntry patternEntry) {
        _patternEntryList.Remove(patternEntry);
        patternEntry.OnDelete.RemoveAllListeners();
        _currentPatternResource.SequenceList.Remove(patternEntry.Sequence);
        Destroy(patternEntry.gameObject);
    }

    private Vector3 GetSpawnPointPosition(EnemySpawnPoint.TypeEnum type) {
        foreach (EnemySpawnPoint spawnPoint in _spawnPoints) {
            if (spawnPoint.Type == type) {
                return spawnPoint.transform.position;
            }
        }
        return Vector3.zero;
    }
    
}
