using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PatternEntry : MonoBehaviour {

    public UnityEvent<PatternEntry> OnDelete;

    [SerializeField] private Text _patternLabel;
    [SerializeField] private InputField _patternTime;
    [SerializeField] private InputField _patternAngle;
    [SerializeField] private InputField _patternSpeed;

    private MovementPatternResource.Sequence _sequence;
    public MovementPatternResource.Sequence Sequence => _sequence;

    public void Initialize(MovementPatternResource.Sequence sequence) {
        _sequence = sequence;

        _patternTime.text = sequence.TimeInSeconds.ToString();
        _patternAngle.text = sequence.AngleInDegrees.ToString();
        _patternSpeed.text = sequence.SpeedMultiplier.ToString();

        _patternTime.onValueChanged.AddListener(SaveValues);
        _patternAngle.onValueChanged.AddListener(SaveValues);
        _patternSpeed.onValueChanged.AddListener(SaveValues);
    }

    public void SaveValues(string value) {
        _sequence.TimeInSeconds = float.Parse(_patternTime.text);
        _sequence.AngleInDegrees = float.Parse(_patternAngle.text);
        _sequence.SpeedMultiplier = float.Parse(_patternSpeed.text);
    }

    public void OnDeleteEntry() {
        _patternTime.onValueChanged.RemoveAllListeners();
        _patternAngle.onValueChanged.RemoveAllListeners();
        _patternSpeed.onValueChanged.RemoveAllListeners();
        OnDelete.Invoke(this);
    }
    
}