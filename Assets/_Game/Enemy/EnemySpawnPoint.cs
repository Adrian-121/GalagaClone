using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour {

    public enum TypeEnum {
        TOP_CENTER = 0,
        TOP_LEFT = 1,
        TOP_RIGHT = 2,

        LEFT_TOP = 3,
        LEFT_CENTER = 4,
        LEFT_BOTTOM = 5,

        RIGHT_TOP = 6,
        RIGHT_CENTER = 7,
        RIGHT_BOTTOM = 8
    }

    [SerializeField] private TypeEnum _type;
    public TypeEnum Type => _type;

}