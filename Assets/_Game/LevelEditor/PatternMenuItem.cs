using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PatternMenuItem : MonoBehaviour {

    public UnityEvent<MovementPatternResource> OnEdit;

    [SerializeField] private Text _patternName;
    [SerializeField] private Text _patternStartPosition;

    [HideInInspector] public MovementPatternResource PatternResource;

    public void Initialize(MovementPatternResource patternResource) {
        PatternResource = patternResource;
        _patternName.text = patternResource.Name;
        _patternStartPosition.text = ((EnemySpawnPoint.TypeEnum)patternResource.Spawner).ToString();
    }

    public void OnEditPressed() {
        OnEdit.Invoke(PatternResource);
    }

}